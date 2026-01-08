using CinemaTicket.Application.Features.Halls.Commands.DeleteHall;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Halls;

public class DeleteHallCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingHall_DeletesAndSavesChanges()
    {
        // ARRANGE
        var hall = new Hall
        {
            Id = Guid.NewGuid(),
            CinemaId = Guid.NewGuid(),
            Name = "To Delete",
            Rows = 5,
            SeatsPerRow = 10,
            TotalCapacity = 50
        };

        var repo = new FakeHallRepository();
        repo.Seed(hall);

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), repo);
        var handler = new DeleteHallCommandHandler(uow);

        // ACT
        await handler.Handle(new DeleteHallCommand(hall.Id), CancellationToken.None);

        // ASSERT
        Assert.Equal(1, repo.DeleteCalls);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ExistingHall_RemovesHallFromRepository()
    {
        // ARRANGE
        var hall = new Hall
        {
            Id = Guid.NewGuid(),
            CinemaId = Guid.NewGuid(),
            Name = "To Delete",
            Rows = 5,
            SeatsPerRow = 10,
            TotalCapacity = 50
        };

        var repo = new FakeHallRepository();
        repo.Seed(hall);

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), repo);
        var handler = new DeleteHallCommandHandler(uow);

        // ACT
        await handler.Handle(new DeleteHallCommand(hall.Id), CancellationToken.None);

        // ASSERT (state verification)
        var deleted = await uow.Halls.GetByIdAsync(hall.Id, CancellationToken.None);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Handle_NonExistentHall_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new DeleteHallCommandHandler(uow);

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteHallCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
