using MediatR;
using CinemaTicket.Application.Features.Showtimes.DTOs;


public class UpdateShowtimeCommand : IRequest<ShowtimeDto>
{
    public Guid ShowtimeId { get; set; }
    public Guid MovieId { get; set; }
    public Guid HallId { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime EndsAt { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; }
}
