using CinemaTicket.Application.Features.Halls.Commands.UpdateHall;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Halls;

public class UpdateHallCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingHall_SavesChanges()
    {
        // ARRANGE
        var hall = new Hall
        {
            Id = Guid.NewGuid(),
            CinemaId = Guid.NewGuid(),
            Name = "Old",
            Rows = 5,
            SeatsPerRow = 10,
            TotalCapacity = 50
        };

        var repo = new FakeHallRepository();
        repo.Seed(hall);

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), repo);
        var handler = new UpdateHallCommandHandler(uow);

        var command = new UpdateHallCommand(
            Id: hall.Id,
            Name: "New",
            Rows: 6,
            SeatsPerRow: 12
        );

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ExistingHall_UpdatesProperties_AndRecalculatesTotalCapacity()
    {
        // ARRANGE
        var hall = new Hall
        {
            Id = Guid.NewGuid(),
            CinemaId = Guid.NewGuid(),
            Name = "Old Name",
            Rows = 4,
            SeatsPerRow = 8,
            TotalCapacity = 32
        };

        var repo = new FakeHallRepository();
        repo.Seed(hall);

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), repo);
        var handler = new UpdateHallCommandHandler(uow);

        var command = new UpdateHallCommand(
            Id: hall.Id,
            Name: "Updated Name",
            Rows: 7,
            SeatsPerRow: 9
        );

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT (state verification)
        var updated = await uow.Halls.GetByIdAsync(hall.Id, CancellationToken.None);
        Assert.NotNull(updated);

        Assert.Equal("Updated Name", updated!.Name);
        Assert.Equal(7, updated.Rows);
        Assert.Equal(9, updated.SeatsPerRow);
        Assert.Equal(7 * 9, updated.TotalCapacity);
    }

    [Fact]
    public async Task Handle_NonExistentHall_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new UpdateHallCommandHandler(uow);

        var command = new UpdateHallCommand(
            Id: Guid.NewGuid(),
            Name: "X",
            Rows: 1,
            SeatsPerRow: 1
        );

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
