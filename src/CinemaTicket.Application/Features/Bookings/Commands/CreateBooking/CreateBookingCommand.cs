using MediatR;
using CinemaTicket.Application.Features.Bookings.DTOs;

namespace CinemaTicket.Application.Features.Bookings.Commands.CreateBooking;

public sealed record CreateBookingCommand(
    Guid UserId,
    Guid ShowtimeId,
    Guid SeatId,
    string HolderName
) : IRequest<BookingResultDto>;
