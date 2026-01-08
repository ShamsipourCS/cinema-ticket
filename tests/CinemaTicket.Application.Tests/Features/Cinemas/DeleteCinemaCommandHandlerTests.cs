using CinemaTicket.Application.Features.Cinemas.Commands.DeleteCinema;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Cinemas;

public class DeleteCinemaCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCinema_DeletesAndSavesChanges()
    {
        // ARRANGE
        var cinema = new Cinema
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            Address = "Addr",
            City = "City",
            Phone = "123",
            IsActive = true
        };

        var repo = new FakeCinemaRepository();
        repo.Seed(cinema);

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), repo, new FakeHallRepository());
        var handler = new DeleteCinemaCommandHandler(uow);

        // ACT
        await handler.Handle(new DeleteCinemaCommand(cinema.Id), CancellationToken.None);

        // ASSERT
        Assert.Equal(1, repo.DeleteCalls);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ExistingCinema_RemovesCinemaFromRepository()
    {
        // ARRANGE
        var cinema = new Cinema
        {
            Id = Guid.NewGuid(),
            Name = "To Delete",
            Address = "Addr",
            City = "City",
            Phone = "123",
            IsActive = true
        };

        var repo = new FakeCinemaRepository();
        repo.Seed(cinema);

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), repo, new FakeHallRepository());
        var handler = new DeleteCinemaCommandHandler(uow);

        // ACT
        await handler.Handle(new DeleteCinemaCommand(cinema.Id), CancellationToken.None);

        // ASSERT (state verification)
        var deleted = await uow.Cinemas.GetByIdAsync(cinema.Id, CancellationToken.None);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Handle_NonExistentCinema_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new DeleteCinemaCommandHandler(uow);

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteCinemaCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
