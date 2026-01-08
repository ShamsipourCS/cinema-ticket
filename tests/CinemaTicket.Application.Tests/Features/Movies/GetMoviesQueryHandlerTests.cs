using CinemaTicket.Application.Features.Movies.Queries.GetMovies;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities; // adjust if needed
namespace CinemaTicket.Application.Tests.Features.Movies;

public class GetMoviesQueryHandlerTests
{
    [Fact]
    public async Task Handle_NoGenreFilter_ReturnsAllMovies_IncludingInactive()
    {
        // ARRANGE
        var repo = new FakeMovieRepository();
        repo.Seed(
            new Movie { Id = Guid.NewGuid(), Title = "A1", Genre = "Action", IsActive = true },
            new Movie { Id = Guid.NewGuid(), Title = "A2", Genre = "Action", IsActive = true },
            new Movie { Id = Guid.NewGuid(), Title = "D1", Genre = "Drama", IsActive = true },
            new Movie { Id = Guid.NewGuid(), Title = "Inactive", Genre = "Drama", IsActive = false }
        );

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMoviesQueryHandler(uow);

        // ACT (use a big page size so paging doesn't hide items)
        var result = await handler.Handle(new GetMoviesQuery(PageNumber: 1, PageSize: 20), CancellationToken.None);

        // ASSERT
        Assert.Equal(4, result.Count);
    }

    [Fact]
    public async Task Handle_GenreFilter_ReturnsAllMatchingGenre_IncludingInactive()
    {
        // ARRANGE
        var repo = new FakeMovieRepository();
        repo.Seed(
            new Movie { Id = Guid.NewGuid(), Title = "Action1", Genre = "Action", IsActive = true },
            new Movie { Id = Guid.NewGuid(), Title = "Drama1", Genre = "Drama", IsActive = true },
            new Movie { Id = Guid.NewGuid(), Title = "ActionInactive", Genre = "Action", IsActive = false }
        );

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMoviesQueryHandler(uow);

        // ACT
        var result = await handler.Handle(new GetMoviesQuery(Genre: "Action", PageNumber: 1, PageSize: 20), CancellationToken.None);

        // ASSERT
        Assert.Equal(2, result.Count);
        Assert.All(result, m => Assert.Equal("Action", m.Genre));
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 5)]
    public async Task Handle_Pagination_ReturnsCorrectPage(int pageNumber, int pageSize)
    {
        // ARRANGE
        var repo = new FakeMovieRepository();
        repo.Seed(Enumerable.Range(1, 12)
            .Select(i => new Movie
            {
                Id = Guid.NewGuid(),
                Title = $"Movie {i:00}", // avoids "Movie 10" containing "Movie 1"
                Genre = "Any",
                IsActive = true
            })
            .ToArray());

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMoviesQueryHandler(uow);

        // ACT
        var result = await handler.Handle(new GetMoviesQuery(PageNumber: pageNumber, PageSize: pageSize), CancellationToken.None);

        // ASSERT
        Assert.Equal(pageSize, result.Count);

        var expected = pageNumber == 1
            ? new[] { "Movie 01", "Movie 02", "Movie 03", "Movie 04", "Movie 05" }
            : new[] { "Movie 06", "Movie 07", "Movie 08", "Movie 09", "Movie 10" };

        Assert.Equal(expected, result.Select(x => x.Title).ToArray());
    }

    [Fact]
    public async Task Handle_EmptyRepo_ReturnsEmptyList()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMoviesQueryHandler(uow);

        // ACT
        var result = await handler.Handle(new GetMoviesQuery(PageNumber: 1, PageSize: 10), CancellationToken.None);

        // ASSERT
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_GenreFilter_NoMatches_ReturnsEmptyList()
    {
        // ARRANGE
        var repo = new FakeMovieRepository();
        repo.Seed(new Movie { Id = Guid.NewGuid(), Title = "Drama1", Genre = "Drama", IsActive = true });

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMoviesQueryHandler(uow);

        // ACT
        var result = await handler.Handle(new GetMoviesQuery(Genre: "Action", PageNumber: 1, PageSize: 10), CancellationToken.None);

        // ASSERT
        Assert.Empty(result);
    }
}
