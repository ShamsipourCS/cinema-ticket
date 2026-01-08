using CinemaTicket.Application.Features.Cinemas.Commands.UpdateCinema;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Cinemas;

public class UpdateCinemaCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCinema_SavesChanges()
    {
        // ARRANGE
        var cinema = new Cinema
        {
            Id = Guid.NewGuid(),
            Name = "Old",
            Address = "Old Address",
            City = "Old City",
            Phone = "000",
            IsActive = true
        };

        var repo = new FakeCinemaRepository();
        repo.Seed(cinema);

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), repo, new FakeHallRepository());
        var handler = new UpdateCinemaCommandHandler(uow);

        var command = new UpdateCinemaCommand(
            Id: cinema.Id,
            Name: "New",
            Address: "New Address",
            City: "New City",
            Phone: "111",
            IsActive: false
        );

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ExistingCinema_PropertiesUpdatedCorrectly()
    {
        // ARRANGE
        var cinema = new Cinema
        {
            Id = Guid.NewGuid(),
            Name = "Old Name",
            Address = "Old Addr",
            City = "Brussels",
            Phone = "123",
            IsActive = true
        };

        var repo = new FakeCinemaRepository();
        repo.Seed(cinema);

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), repo, new FakeHallRepository());
        var handler = new UpdateCinemaCommandHandler(uow);

        var command = new UpdateCinemaCommand(
            Id: cinema.Id,
            Name: "Updated Name",
            Address: "Updated Addr",
            City: "Antwerp",
            Phone: "+32 555",
            IsActive: false
        );

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT (state verification)
        var updated = await uow.Cinemas.GetByIdAsync(cinema.Id, CancellationToken.None);
        Assert.NotNull(updated);

        Assert.Equal("Updated Name", updated!.Name);
        Assert.Equal("Updated Addr", updated.Address);
        Assert.Equal("Antwerp", updated.City);
        Assert.Equal("+32 555", updated.Phone);
        Assert.False(updated.IsActive);
    }

    [Fact]
    public async Task Handle_NonExistentCinema_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new UpdateCinemaCommandHandler(uow);

        var command = new UpdateCinemaCommand(
            Id: Guid.NewGuid(),
            Name: "X",
            Address: "X",
            City: "X",
            Phone: "X",
            IsActive: true
        );

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
