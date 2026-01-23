using MediatR;
using CinemaTicket.Application.Features.Bookings.DTOs;

namespace CinemaTicket.Application.Features.Bookings.Commands.ConfirmBooking;

/// <summary>
/// Command to confirm a booking after successful Stripe payment.
/// Links payment to ticket and creates confirmed booking.
/// </summary>
public sealed record ConfirmBookingCommand(
    string StripePaymentIntentId,
    Guid UserId,
    Guid ShowtimeId,
    Guid SeatId,
    string HolderName
) : IRequest<BookingResultDto>;
