using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Features.Cinemas.Commands.CreateCinema;
using CinemaTicket.Application.Tests.Fakes;
using Xunit;

namespace CinemaTicket.Application.Tests.Features.Cinemas;

public class CreateCinemaCommandHandlerTests
{
    [Fact]
    public async Task Handle_AddsCinema_AndCallsSaveChanges()
    {
        var moviesRepo = new FakeMovieRepository();
        var cinemasRepo = new FakeCinemaRepository();
        var hallsRepo = new FakeHallRepository();

        using var uow = new FakeUnitOfWork(moviesRepo, cinemasRepo, hallsRepo);
        var handler = new CreateCinemaCommandHandler(uow);

        var cmd = new CreateCinemaCommand(
            Name: "Cinema A",
            Address: "Addr",
            City: "Tehran",
            Phone: "123",
            IsActive: true
        );

        var id = await handler.Handle(cmd, CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
        Assert.Equal(1, cinemasRepo.AddCalls);
        Assert.Equal(1, uow.SaveChangesCalls);

        var saved = await cinemasRepo.GetByIdAsync(id, CancellationToken.None);
        Assert.NotNull(saved);
        Assert.Equal("Cinema A", saved!.Name);
        Assert.Equal("Tehran", saved.City);
    }
}
