using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Enums;
using CinemaTicket.Domain.Interfaces;

namespace CinemaTicket.Application.Features.Payments.Commands.ProcessWebhook;

/// <summary>
/// Handler for processing Stripe webhook events.
/// Verifies webhook signatures, parses events, and updates payment/ticket status accordingly.
/// </summary>
public sealed class ProcessWebhookCommandHandler : IRequestHandler<ProcessWebhookCommand, bool>
{
    private readonly IApplicationDbContext _db;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStripePaymentService _stripeService;
    private readonly ILogger<ProcessWebhookCommandHandler> _logger;

    // Supported webhook event types
    private const string PaymentIntentSucceeded = "payment_intent.succeeded";
    private const string PaymentIntentPaymentFailed = "payment_intent.payment_failed";
    private const string PaymentIntentCanceled = "payment_intent.canceled";

    public ProcessWebhookCommandHandler(
        IApplicationDbContext db,
        IUnitOfWork unitOfWork,
        IStripePaymentService stripeService,
        ILogger<ProcessWebhookCommandHandler> logger)
    {
        _db = db;
        _unitOfWork = unitOfWork;
        _stripeService = stripeService;
        _logger = logger;
    }

    public async Task<bool> Handle(ProcessWebhookCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing Stripe webhook event");

        // Step 1: Verify webhook signature and extract event details
        string eventId, eventType;
        string? paymentIntentId;

        try
        {
            (eventId, eventType, paymentIntentId) = _stripeService.VerifyWebhookSignature(
                request.Json,
                request.StripeSignatureHeader);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, "Webhook signature verification failed");
            throw;
        }

        _logger.LogInformation(
            "Webhook signature verified: Event {EventId}, Type {EventType}",
            eventId, eventType);

        // Step 2: Check if we handle this event type
        if (!IsHandledEventType(eventType))
        {
            _logger.LogDebug("Ignoring unhandled event type {EventType}", eventType);
            return true; // Return success to acknowledge receipt
        }

        // Step 3: Check if PaymentIntent ID is present
        if (string.IsNullOrEmpty(paymentIntentId))
        {
            _logger.LogWarning("Event {EventId} does not contain a PaymentIntent ID", eventId);
            return true; // Return success - not an error
        }

        _logger.LogInformation(
            "Processing event {EventType} for payment intent {PaymentIntentId}",
            eventType, paymentIntentId);

        // Step 4: Find payment in database
        var payment = await _unitOfWork.Payments.GetByStripePaymentIntentIdAsync(
            paymentIntentId, cancellationToken);

        if (payment is null)
        {
            _logger.LogWarning(
                "Payment not found for intent {PaymentIntentId}, may be duplicate event or race condition",
                paymentIntentId);
            return true; // Return success for idempotency
        }

        // Step 5: Process based on event type
        try
        {
            switch (eventType)
            {
                case PaymentIntentSucceeded:
                    await HandlePaymentSucceededAsync(payment, cancellationToken);
                    break;

                case PaymentIntentPaymentFailed:
                    await HandlePaymentFailedAsync(payment, cancellationToken);
                    break;

                case PaymentIntentCanceled:
                    await HandlePaymentCanceledAsync(payment, cancellationToken);
                    break;
            }

            _logger.LogInformation(
                "Successfully processed webhook event {EventId} for payment {PaymentId}",
                eventId, payment.Id);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing webhook event {EventId} for payment intent {PaymentIntentId}",
                eventId, paymentIntentId);
            throw;
        }
    }

    private async Task HandlePaymentSucceededAsync(
        Domain.Entities.Payment payment,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling payment_intent.succeeded for payment {PaymentId}", payment.Id);

        // Idempotency: Check if already processed
        if (payment.Status == PaymentStatus.Success)
        {
            _logger.LogDebug("Payment {PaymentId} already marked as Success, skipping update", payment.Id);
            return;
        }

        // Update payment status
        payment.Status = PaymentStatus.Success;
        await _unitOfWork.Payments.UpdateAsync(payment);

        // If payment linked to ticket, confirm ticket
        if (payment.TicketId != Guid.Empty)
        {
            var ticket = await _db.Tickets
                .FirstOrDefaultAsync(t => t.Id == payment.TicketId, cancellationToken);

            if (ticket is not null && ticket.Status == TicketStatus.Pending)
            {
                ticket.Status = TicketStatus.Confirmed;
                _db.Tickets.Update(ticket);
                _logger.LogInformation(
                    "Updated ticket {TicketId} status to Confirmed",
                    ticket.Id);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Payment {PaymentId} marked as Success", payment.Id);
    }

    private async Task HandlePaymentFailedAsync(
        Domain.Entities.Payment payment,
        CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Handling payment_intent.payment_failed for payment {PaymentId}",
            payment.Id);

        // Idempotency: Check if already processed
        if (payment.Status == PaymentStatus.Failed)
        {
            _logger.LogDebug("Payment {PaymentId} already marked as Failed, skipping update", payment.Id);
            return;
        }

        // Update payment status
        payment.Status = PaymentStatus.Failed;
        await _unitOfWork.Payments.UpdateAsync(payment);

        // If payment linked to ticket, cancel ticket
        if (payment.TicketId != Guid.Empty)
        {
            var ticket = await _db.Tickets
                .FirstOrDefaultAsync(t => t.Id == payment.TicketId, cancellationToken);

            if (ticket is not null && ticket.Status != TicketStatus.Cancelled)
            {
                ticket.Status = TicketStatus.Cancelled;
                _db.Tickets.Update(ticket);
                _logger.LogInformation(
                    "Cancelled ticket {TicketId} due to payment failure",
                    ticket.Id);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Payment {PaymentId} marked as Failed", payment.Id);
    }

    private async Task HandlePaymentCanceledAsync(
        Domain.Entities.Payment payment,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling payment_intent.canceled for payment {PaymentId}", payment.Id);

        // Idempotency: Check if already processed
        if (payment.Status == PaymentStatus.Refunded)
        {
            _logger.LogDebug("Payment {PaymentId} already marked as Refunded, skipping update", payment.Id);
            return;
        }

        // Update payment status
        payment.Status = PaymentStatus.Refunded;
        await _unitOfWork.Payments.UpdateAsync(payment);

        // If payment linked to ticket, cancel ticket
        if (payment.TicketId != Guid.Empty)
        {
            var ticket = await _db.Tickets
                .FirstOrDefaultAsync(t => t.Id == payment.TicketId, cancellationToken);

            if (ticket is not null && ticket.Status != TicketStatus.Cancelled)
            {
                ticket.Status = TicketStatus.Cancelled;
                _db.Tickets.Update(ticket);
                _logger.LogInformation(
                    "Cancelled ticket {TicketId} due to payment cancellation",
                    ticket.Id);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Payment {PaymentId} marked as Refunded", payment.Id);
    }

    private static bool IsHandledEventType(string eventType)
    {
        return eventType switch
        {
            PaymentIntentSucceeded => true,
            PaymentIntentPaymentFailed => true,
            PaymentIntentCanceled => true,
            _ => false
        };
    }
}
