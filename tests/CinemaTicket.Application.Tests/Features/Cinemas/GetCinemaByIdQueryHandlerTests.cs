using CinemaTicket.Application.Features.Cinemas.Queries.GetCinemaById;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Cinemas;

public class GetCinemaByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingCinema_ReturnsCorrectDto()
    {
        // ARRANGE
        var cinema = new Cinema
        {
            Id = Guid.NewGuid(),
            Name = "Existing Cinema",
            Address = "Addr 1",
            City = "Brussels",
            Phone = "123",
            IsActive = true
        };

        var cinemaRepo = new FakeCinemaRepository();
        cinemaRepo.Seed(cinema);

        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            cinemaRepo,
            new FakeHallRepository());

        var handler = new GetCinemaByIdQueryHandler(uow);

        // ACT
        var dto = await handler.Handle(new GetCinemaByIdQuery(cinema.Id), CancellationToken.None);

        // ASSERT
        Assert.NotNull(dto);
        Assert.Equal(cinema.Id, dto.Id);
        Assert.Equal(cinema.Name, dto.Name);
        Assert.Equal(cinema.City, dto.City);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public async Task Handle_NonExistentCinema_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new GetCinemaByIdQueryHandler(uow);

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new GetCinemaByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExistingCinema_MapsAllPropertiesCorrectly()
    {
        // ARRANGE
        var cinema = new Cinema
        {
            Id = Guid.NewGuid(),
            Name = "Map Me",
            Address = "Street 99",
            City = "Antwerp",
            Phone = "+32 555 000",
            IsActive = false
        };

        var cinemaRepo = new FakeCinemaRepository();
        cinemaRepo.Seed(cinema);

        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            cinemaRepo,
            new FakeHallRepository());

        var handler = new GetCinemaByIdQueryHandler(uow);

        // ACT
        var dto = await handler.Handle(new GetCinemaByIdQuery(cinema.Id), CancellationToken.None);

        // ASSERT
        Assert.Equal(cinema.Id, dto.Id);
        Assert.Equal(cinema.Name, dto.Name);
        Assert.Equal(cinema.Address, dto.Address);
        Assert.Equal(cinema.City, dto.City);
        Assert.Equal(cinema.Phone, dto.Phone);
        Assert.Equal(cinema.IsActive, dto.IsActive);
    }
}
