using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Application.Features.Payments.DTOs;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Enums;
using CinemaTicket.Domain.Exceptions;
using CinemaTicket.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace CinemaTicket.Application.Features.Payments.Commands.CreatePaymentIntent;

/// <summary>
/// Handler for creating a Stripe payment intent.
/// Requires authenticated user context.
/// </summary>
public sealed class CreatePaymentIntentCommandHandler : IRequestHandler<CreatePaymentIntentCommand, PaymentIntentResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStripePaymentService _stripePaymentService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreatePaymentIntentCommandHandler(
        IUnitOfWork unitOfWork,
        IStripePaymentService stripePaymentService,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _stripePaymentService = stripePaymentService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<PaymentIntentResponseDto> Handle(CreatePaymentIntentCommand request, CancellationToken cancellationToken)
    {
        // Get authenticated user ID from HTTP context
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new Domain.Exceptions.UnauthorizedAccessException("User is not authenticated.");
        }

        // Build metadata for Stripe payment intent
        var metadata = request.Metadata ?? new Dictionary<string, string>();
        metadata["UserId"] = userId.ToString();
        metadata["Purpose"] = "TicketBooking";
        metadata["RequestId"] = Guid.NewGuid().ToString();

        // Create payment intent with Stripe
        var currency = request.Currency ?? "usd";
        var paymentIntent = await _stripePaymentService.CreatePaymentIntentAsync(
            request.Amount,
            currency,
            metadata);

        // Create payment record in database
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            StripePaymentIntentId = paymentIntent.PaymentIntentId,
            Amount = request.Amount / 100m, // Convert cents to dollars
            Currency = currency,
            Status = PaymentStatus.Pending
            // TicketId will be set later when booking is completed
        };

        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Return payment intent response
        return new PaymentIntentResponseDto(
            PaymentIntentId: paymentIntent.PaymentIntentId,
            ClientSecret: paymentIntent.ClientSecret,
            Amount: request.Amount,
            Currency: currency
        );
    }
}
