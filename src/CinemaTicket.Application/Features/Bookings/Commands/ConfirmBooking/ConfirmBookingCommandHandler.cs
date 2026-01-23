using System.Transactions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Bookings.DTOs;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using CinemaTicket.Domain.Interfaces;

namespace CinemaTicket.Application.Features.Bookings.Commands.ConfirmBooking;

/// <summary>
/// Handler for confirming bookings after successful Stripe payment.
/// Validates payment status, creates confirmed ticket, and links payment to ticket.
/// </summary>
public sealed class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, BookingResultDto>
{
    private readonly IApplicationDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStripePaymentService _stripeService;
    private readonly ILogger<ConfirmBookingCommandHandler> _logger;

    public ConfirmBookingCommandHandler(
        IApplicationDbContext db,
        IUnitOfWork unitOfWork,
        IStripePaymentService stripeService,
        ILogger<ConfirmBookingCommandHandler> logger)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _stripeService = stripeService;
        _logger = logger;
    }

    public async Task<BookingResultDto> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Confirming booking for payment intent {PaymentIntentId}, user {UserId}, showtime {ShowtimeId}, seat {SeatId}",
            request.StripePaymentIntentId, request.UserId, request.ShowtimeId, request.SeatId);

        // Step 1: Validate Payment exists and check status
        var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(
            request.StripePaymentIntentId, cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning("Payment not found for intent {PaymentIntentId}", request.StripePaymentIntentId);
            throw new KeyNotFoundException($"Payment not found for intent {request.StripePaymentIntentId}");
        }

        // Verify payment status from Stripe
        string paymentStatus;
        try
        {
            paymentStatus = await _stripeService.GetPaymentIntentStatusAsync(
                request.StripePaymentIntentId, cancellationToken);
            _logger.LogDebug("Payment intent {PaymentIntentId} status: {Status}",
                request.StripePaymentIntentId, paymentStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve payment status from Stripe for intent {PaymentIntentId}",
                request.StripePaymentIntentId);
            throw new InvalidOperationException("Failed to verify payment status with Stripe", ex);
        }

        // Check if payment succeeded
        if (paymentStatus != "succeeded" && paymentStatus != "processing")
        {
            _logger.LogWarning(
                "Payment intent {PaymentIntentId} has status {Status}, cannot confirm booking",
                request.StripePaymentIntentId, paymentStatus);
            throw new InvalidOperationException(
                $"Payment has not succeeded. Current status: {paymentStatus}");
        }

        // Step 2: Create ticket with Serializable transaction
        var txOptions = new TransactionOptions
        {
            IsolationLevel = System.Transactions.IsolationLevel.Serializable,
            Timeout = TimeSpan.FromSeconds(30)
        };

        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            txOptions,
            TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            // 2.1: Validate Showtime exists & active
            var showtime = await _db.Showtimes
                .FirstOrDefaultAsync(s => s.Id == request.ShowtimeId, cancellationToken);

            if (showtime is null)
            {
                _logger.LogWarning("Showtime {ShowtimeId} not found", request.ShowtimeId);
                throw new KeyNotFoundException("Showtime not found");
            }

            if (!showtime.IsActive)
            {
                _logger.LogWarning("Showtime {ShowtimeId} is not active", request.ShowtimeId);
                throw new InvalidOperationException("Showtime is not active");
            }

            // 2.2: Validate Seat exists & belongs to showtime hall
            var seat = await _db.Seats
                .FirstOrDefaultAsync(s => s.Id == request.SeatId, cancellationToken);

            if (seat is null)
            {
                _logger.LogWarning("Seat {SeatId} not found", request.SeatId);
                throw new KeyNotFoundException("Seat not found");
            }

            if (seat.HallId != showtime.HallId)
            {
                _logger.LogWarning(
                    "Seat {SeatId} (Hall {SeatHallId}) does not belong to Showtime {ShowtimeId} hall {ShowtimeHallId}",
                    request.SeatId, seat.HallId, request.ShowtimeId, showtime.HallId);
                throw new InvalidOperationException("Seat does not belong to the showtime hall");
            }

            // 2.3: Ensure seat is not already reserved/confirmed
            var existsActiveTicket = await _db.Tickets.AnyAsync(t =>
                t.ShowtimeId == request.ShowtimeId &&
                t.SeatId == request.SeatId &&
                t.Status != TicketStatus.Cancelled,
                cancellationToken);

            if (existsActiveTicket)
            {
                _logger.LogWarning(
                    "Seat {SeatId} for Showtime {ShowtimeId} is already reserved",
                    request.SeatId, request.ShowtimeId);
                throw new InvalidOperationException("Seat is already reserved");
            }

            // 2.4: Calculate price and validate against payment amount
            var calculatedPrice = CalculatePrice(showtime.BasePrice, seat.PriceMultiplier);

            // Allow 1 cent tolerance for rounding differences
            var priceDifference = Math.Abs(calculatedPrice - payment.Amount);
            if (priceDifference > 0.01m)
            {
                _logger.LogWarning(
                    "Price mismatch: Calculated {CalculatedPrice}, Payment {PaymentAmount}, Difference {Difference}",
                    calculatedPrice, payment.Amount, priceDifference);
                throw new InvalidOperationException(
                    $"Price mismatch: Expected {calculatedPrice:C}, but payment was {payment.Amount:C}");
            }

            // 2.5: Create Ticket with Confirmed status (NOT Pending!)
            var ticket = new Ticket
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ShowtimeId = request.ShowtimeId,
                SeatId = request.SeatId,
                HolderName = request.HolderName.Trim(),
                TicketNumber = GenerateTicketNumber(),
                Price = calculatedPrice,
                Status = TicketStatus.Confirmed // Immediately confirmed since payment succeeded
            };

            _db.Tickets.Add(ticket);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Created confirmed ticket {TicketId} with number {TicketNumber}",
                ticket.Id, ticket.TicketNumber);

            // Step 3: Link Payment to Ticket and update payment status
            payment.TicketId = ticket.Id;
            payment.Status = PaymentStatus.Success;

            // Update payment through repository
            await _unitOfWork.Payments.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Linked payment {PaymentId} to ticket {TicketId} and updated status to Success",
                payment.Id, ticket.Id);

            // Commit transaction
            scope.Complete();

            _logger.LogInformation(
                "Successfully confirmed booking: Ticket {TicketId}, Payment {PaymentId}",
                ticket.Id, payment.Id);

            return new BookingResultDto(
                TicketId: ticket.Id,
                TicketNumber: ticket.TicketNumber,
                ShowtimeId: ticket.ShowtimeId,
                SeatId: ticket.SeatId,
                Price: ticket.Price,
                Status: ticket.Status
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transaction failed during booking confirmation for payment intent {PaymentIntentId}",
                request.StripePaymentIntentId);
            throw;
        }
    }

    private static decimal CalculatePrice(decimal basePrice, decimal multiplier)
    {
        var price = basePrice * multiplier;

        // Ensure non-negative price
        if (price < 0)
            price = 0;

        return decimal.Round(price, 2, MidpointRounding.AwayFromZero);
    }

    private static string GenerateTicketNumber()
    {
        // Format: yyyyMMdd-{10 uppercase alphanumeric chars}
        var suffix = Guid.NewGuid().ToString("N")[..10].ToUpperInvariant();
        return $"{DateTime.UtcNow:yyyyMMdd}-{suffix}";
    }
}
