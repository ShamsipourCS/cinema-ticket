
using CinemaTicket.Application.Features.Cinemas.Queries.GetCinemas;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Cinemas;

public class GetCinemasQueryHandlerTests
{
    [Fact]
    public async Task Handle_NoCityFilter_ReturnsAllCinemas_IncludingInactive()
    {
        // ARRANGE
        var repo = new FakeCinemaRepository();
        repo.Seed(
            new Cinema { Id = Guid.NewGuid(), Name = "C1", City = "Brussels", IsActive = true },
            new Cinema { Id = Guid.NewGuid(), Name = "C2", City = "Brussels", IsActive = false },
            new Cinema { Id = Guid.NewGuid(), Name = "C3", City = "Antwerp", IsActive = true },
            new Cinema { Id = Guid.NewGuid(), Name = "C4", City = "Ghent", IsActive = true }
        );

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), repo, new FakeHallRepository());
        var handler = new GetCinemasQueryHandler(uow);

        // ACT (big page size so paging doesn't hide items)
        var result = await handler.Handle(new GetCinemasQuery(PageNumber: 1, PageSize: 20), CancellationToken.None);

        // ASSERT
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public async Task Handle_CityFilter_ReturnsOnlyMatchingCity()
    {
        // ARRANGE
        var repo = new FakeCinemaRepository();
        repo.Seed(
            new Cinema { Id = Guid.NewGuid(), Name = "B1", City = "Brussels", IsActive = true },
            new Cinema { Id = Guid.NewGuid(), Name = "B2", City = "Brussels", IsActive = false },
            new Cinema { Id = Guid.NewGuid(), Name = "A1", City = "Antwerp", IsActive = true }
        );

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), repo, new FakeHallRepository());
        var handler = new GetCinemasQueryHandler(uow);

        // ACT
        var result = await handler.Handle(new GetCinemasQuery(City: "Brussels", PageNumber: 1, PageSize: 20), CancellationToken.None);

        // ASSERT
        Assert.Equal(2, result.Count);
        Assert.All(result, c => Assert.Equal("Brussels", c.City));
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    public async Task Handle_Pagination_ReturnsCorrectPage(int pageNumber, int pageSize)
    {
        // ARRANGE
        var repo = new FakeCinemaRepository();
        repo.Seed(Enumerable.Range(1, 12)
            .Select(i => new Cinema
            {
                Id = Guid.NewGuid(),
                Name = $"Cinema {i:00}",
                Address = "Addr",
                City = "City",
                Phone = "Phone",
                IsActive = true
            })
            .ToArray());

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), repo, new FakeHallRepository());
        var handler = new GetCinemasQueryHandler(uow);

        // ACT
        var result = await handler.Handle(new GetCinemasQuery(PageNumber: pageNumber, PageSize: pageSize), CancellationToken.None);

        // ASSERT
        Assert.Equal(pageSize, result.Count);

        var expected = pageNumber == 1
            ? new[] { "Cinema 01", "Cinema 02", "Cinema 03", "Cinema 04", "Cinema 05" }
            : new[] { "Cinema 06", "Cinema 07", "Cinema 08", "Cinema 09", "Cinema 10" };

        Assert.Equal(expected, result.Select(x => x.Name).ToArray());
    }

    [Fact]
    public async Task Handle_EmptyRepo_ReturnsEmptyList()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetCinemasQueryHandler(uow);

        // ACT
        var result = await handler.Handle(new GetCinemasQuery(PageNumber: 1, PageSize: 10), CancellationToken.None);

        // ASSERT
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_CityFilter_NoMatches_ReturnsEmptyList()
    {
        // ARRANGE
        var repo = new FakeCinemaRepository();
        repo.Seed(new Cinema { Id = Guid.NewGuid(), Name = "A1", City = "Antwerp", IsActive = true });

        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), repo, new FakeHallRepository());
        var handler = new GetCinemasQueryHandler(uow);

        // ACT
        var result = await handler.Handle(new GetCinemasQuery(City: "Brussels", PageNumber: 1, PageSize: 10), CancellationToken.None);

        // ASSERT
        Assert.Empty(result);
    }
}
