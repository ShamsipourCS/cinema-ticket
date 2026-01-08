using CinemaTicket.Application.Features.Halls.Queries.GetHallsByCinema;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Halls;

public class GetHallsByCinemaQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsOnlyHallsForSpecifiedCinema()
    {
        // ARRANGE
        var cinemaId = Guid.NewGuid();
        var otherCinemaId = Guid.NewGuid();

        var repo = new FakeHallRepository();
        repo.Seed(
            new Hall { Id = Guid.NewGuid(), CinemaId = cinemaId, Name = "H1", Rows = 5, SeatsPerRow = 10, TotalCapacity = 50 },
            new Hall { Id = Guid.NewGuid(), CinemaId = cinemaId, Name = "H2", Rows = 6, SeatsPerRow = 10, TotalCapacity = 60 },
            new Hall { Id = Guid.NewGuid(), CinemaId = otherCinemaId, Name = "Other", Rows = 7, SeatsPerRow = 10, TotalCapacity = 70 }
        );

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), repo);
        var handler = new GetHallsByCinemaQueryHandler(uow);

        // ACT
        var result = await handler.Handle(
            new GetHallsByCinemaQuery(CinemaId: cinemaId, PageNumber: 1, PageSize: 20),
            CancellationToken.None);

        // ASSERT
        Assert.Equal(2, result.Count);
        Assert.All(result, h => Assert.Equal(cinemaId, h.CinemaId));
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    public async Task Handle_Pagination_ReturnsCorrectPage(int pageNumber, int pageSize)
    {
        // ARRANGE
        var cinemaId = Guid.NewGuid();
        var repo = new FakeHallRepository();

        repo.Seed(Enumerable.Range(1, 12)
            .Select(i => new Hall
            {
                Id = Guid.NewGuid(),
                CinemaId = cinemaId,
                Name = $"Hall {i:00}",
                Rows = 10,
                SeatsPerRow = 10,
                TotalCapacity = 100
            })
            .ToArray());

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), repo);
        var handler = new GetHallsByCinemaQueryHandler(uow);

        // ACT
        var result = await handler.Handle(
            new GetHallsByCinemaQuery(CinemaId: cinemaId, PageNumber: pageNumber, PageSize: pageSize),
            CancellationToken.None);

        // ASSERT
        Assert.Equal(pageSize, result.Count);

        var expected = pageNumber == 1
            ? new[] { "Hall 01", "Hall 02", "Hall 03", "Hall 04", "Hall 05" }
            : new[] { "Hall 06", "Hall 07", "Hall 08", "Hall 09", "Hall 10" };

        Assert.Equal(expected, result.Select(x => x.Name).ToArray());
    }

    [Fact]
    public async Task Handle_NoHallsForCinema_ReturnsEmptyList()
    {
        // ARRANGE
        var cinemaId = Guid.NewGuid();

        var repo = new FakeHallRepository();
        repo.Seed(
            new Hall { Id = Guid.NewGuid(), CinemaId = Guid.NewGuid(), Name = "Other", Rows = 5, SeatsPerRow = 5, TotalCapacity = 25 }
        );

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), repo);
        var handler = new GetHallsByCinemaQueryHandler(uow);

        // ACT
        var result = await handler.Handle(
            new GetHallsByCinemaQuery(CinemaId: cinemaId, PageNumber: 1, PageSize: 10),
            CancellationToken.None);

        // ASSERT
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_EmptyRepo_ReturnsEmptyList()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetHallsByCinemaQueryHandler(uow);

        // ACT
        var result = await handler.Handle(
            new GetHallsByCinemaQuery(CinemaId: Guid.NewGuid(), PageNumber: 1, PageSize: 10),
            CancellationToken.None);

        // ASSERT
        Assert.Empty(result);
    }
}
