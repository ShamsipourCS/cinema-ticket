using CinemaTicket.Application.Features.Movies.Queries.GetMovieById;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities; // adjust if needed

namespace CinemaTicket.Application.Tests.Features.Movies;

public class GetMovieByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ExistingMovie_ReturnsCorrectDto()
    {
        // ARRANGE
        var existingMovie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Existing Movie",
            Description = "Desc",
            DurationMinutes = 120,
            Genre = "Drama",
            Rating = "PG-13",
            PosterUrl = "http://example.com/poster.jpg",
            ReleaseDate = DateTime.UtcNow.AddDays(5),
            IsActive = true
        };

        var movieRepo = new FakeMovieRepository();
        movieRepo.Seed(existingMovie);

        using var uow = new FakeUnitOfWork(movieRepo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMovieByIdQueryHandler(uow);
        var query = new GetMovieByIdQuery(existingMovie.Id);

        // ACT
        var result = await handler.Handle(query, CancellationToken.None);

        // ASSERT
        Assert.NotNull(result);
        Assert.Equal(existingMovie.Id, result.Id);
        Assert.Equal(existingMovie.Title, result.Title);
        Assert.Equal(existingMovie.Genre, result.Genre);
        Assert.Equal(existingMovie.IsActive, result.IsActive);
    }

    [Fact]
    public async Task Handle_NonExistentMovie_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMovieByIdQueryHandler(uow);
        var query = new GetMovieByIdQuery(Guid.NewGuid());

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(query, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ExistingMovie_MapsAllPropertiesCorrectly()
    {
        // ARRANGE
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Map Me",
            Description = "Full mapping check",
            DurationMinutes = 95,
            Genre = "Action",
            Rating = "R",
            PosterUrl = "http://example.com/x.png",
            ReleaseDate = new DateTime(2030, 1, 1),
            IsActive = true
        };

        var repo = new FakeMovieRepository();
        repo.Seed(movie);

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMovieByIdQueryHandler(uow);

        // ACT
        var dto = await handler.Handle(new GetMovieByIdQuery(movie.Id), CancellationToken.None);

        // ASSERT
        Assert.Equal(movie.Title, dto.Title);
        Assert.Equal(movie.Description, dto.Description);
        Assert.Equal(movie.DurationMinutes, dto.DurationMinutes);
        Assert.Equal(movie.Genre, dto.Genre);
        Assert.Equal(movie.Rating, dto.Rating);
        Assert.Equal(movie.PosterUrl, dto.PosterUrl);
        Assert.Equal(movie.ReleaseDate, dto.ReleaseDate);
        Assert.Equal(movie.IsActive, dto.IsActive);
    }
}
