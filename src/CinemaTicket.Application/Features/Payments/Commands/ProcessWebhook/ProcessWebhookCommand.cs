using MediatR;

namespace CinemaTicket.Application.Features.Payments.Commands.ProcessWebhook;

/// <summary>
/// Command to process Stripe webhook events.
/// Validates webhook signature and updates payment/ticket status based on event type.
/// </summary>
public sealed record ProcessWebhookCommand(
    string Json,
    string StripeSignatureHeader
) : IRequest<bool>;
