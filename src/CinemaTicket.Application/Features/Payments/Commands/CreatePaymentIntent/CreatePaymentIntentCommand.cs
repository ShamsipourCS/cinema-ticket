using System.Collections.Generic;
using CinemaTicket.Application.Features.Payments.DTOs;
using MediatR;

namespace CinemaTicket.Application.Features.Payments.Commands.CreatePaymentIntent;

/// <summary>
/// Command for creating a Stripe payment intent for ticket booking.
/// </summary>
/// <param name="Amount">The payment amount in smallest currency unit (e.g., cents for USD).</param>
/// <param name="Currency">The payment currency (optional, defaults to "usd").</param>
/// <param name="Metadata">Additional metadata for the payment (optional).</param>
public sealed record CreatePaymentIntentCommand(
    long Amount,
    string? Currency,
    Dictionary<string, string>? Metadata
) : IRequest<PaymentIntentResponseDto>;
