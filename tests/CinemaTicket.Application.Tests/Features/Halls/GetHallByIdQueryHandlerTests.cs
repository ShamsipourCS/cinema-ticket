using CinemaTicket.Application.Features.Halls.Queries.GetHallById;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Halls;

public class GetHallByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingHall_ReturnsCorrectDto()
    {
        // ARRANGE
        var hall = new Hall
        {
            Id = Guid.NewGuid(),
            CinemaId = Guid.NewGuid(),
            Name = "Hall A",
            Rows = 10,
            SeatsPerRow = 12,
            TotalCapacity = 120
        };

        var hallRepo = new FakeHallRepository();
        hallRepo.Seed(hall);

        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            hallRepo);

        var handler = new GetHallByIdQueryHandler(uow);

        // ACT
        var dto = await handler.Handle(new GetHallByIdQuery(hall.Id), CancellationToken.None);

        // ASSERT
        Assert.NotNull(dto);
        Assert.Equal(hall.Id, dto.Id);
        Assert.Equal(hall.Name, dto.Name);
        Assert.Equal(hall.TotalCapacity, dto.TotalCapacity);
    }

    [Fact]
    public async Task Handle_NonExistentHall_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new GetHallByIdQueryHandler(uow);

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new GetHallByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExistingHall_MapsAllPropertiesCorrectly()
    {
        // ARRANGE
        var hall = new Hall
        {
            Id = Guid.NewGuid(),
            CinemaId = Guid.NewGuid(),
            Name = "Map Me",
            Rows = 7,
            SeatsPerRow = 9,
            TotalCapacity = 63
        };

        var hallRepo = new FakeHallRepository();
        hallRepo.Seed(hall);

        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            hallRepo);

        var handler = new GetHallByIdQueryHandler(uow);

        // ACT
        var dto = await handler.Handle(new GetHallByIdQuery(hall.Id), CancellationToken.None);

        // ASSERT
        Assert.Equal(hall.Id, dto.Id);
        Assert.Equal(hall.CinemaId, dto.CinemaId);
        Assert.Equal(hall.Name, dto.Name);
        Assert.Equal(hall.Rows, dto.Rows);
        Assert.Equal(hall.SeatsPerRow, dto.SeatsPerRow);
        Assert.Equal(hall.TotalCapacity, dto.TotalCapacity);
    }
}
