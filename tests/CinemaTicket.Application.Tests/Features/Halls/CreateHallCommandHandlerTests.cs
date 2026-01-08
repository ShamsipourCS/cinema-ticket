using CinemaTicket.Application.Features.Halls.Commands.CreateHall;
using CinemaTicket.Application.Tests.Fakes;


namespace CinemaTicket.Application.Tests.Features.Halls;

public class CreateHallCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewHallId_AndSaves()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreateHallCommandHandler(uow);

        var command = new CreateHallCommand(
            CinemaId: Guid.NewGuid(),
            Name: "Hall 1",
            Rows: 10,
            SeatsPerRow: 12
        );

        // ACT
        var hallId = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, hallId);

        var hallRepo = (FakeHallRepository)uow.Halls;
        Assert.Equal(1, hallRepo.AddCalls);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ValidCommand_HallExistsInRepository_WithCorrectFields()
    {
        // ARRANGE
        var hallRepo = new FakeHallRepository();

        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            hallRepo);

        var handler = new CreateHallCommandHandler(uow);

        var cinemaId = Guid.NewGuid();

        var command = new CreateHallCommand(
            CinemaId: cinemaId,
            Name: "Expected Hall",
            Rows: 5,
            SeatsPerRow: 20
        );

        // ACT
        var hallId = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        var created = await uow.Halls.GetByIdAsync(hallId, CancellationToken.None);
        Assert.NotNull(created);

        Assert.Equal(cinemaId, created!.CinemaId);
        Assert.Equal("Expected Hall", created.Name);
        Assert.Equal(5, created.Rows);
        Assert.Equal(20, created.SeatsPerRow);
    }

    [Fact]
    public async Task Handle_ValidCommand_CalculatesTotalCapacityCorrectly()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreateHallCommandHandler(uow);

        var command = new CreateHallCommand(
            CinemaId: Guid.NewGuid(),
            Name: "Capacity Test",
            Rows: 7,
            SeatsPerRow: 13
        );

        // ACT
        var hallId = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        var created = await uow.Halls.GetByIdAsync(hallId, CancellationToken.None);
        Assert.NotNull(created);
        Assert.Equal(7 * 13, created!.TotalCapacity);
    }
}
