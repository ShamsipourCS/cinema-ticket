using CinemaTicket.Application.Features.Movies.Commands.UpdateMovie;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;
namespace CinemaTicket.Application.Tests.Features.Movies;

public class UpdateMovieCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingMovie_SavesChanges()
    {
        // ARRANGE
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Old",
            Genre = "Drama",
            IsActive = true
        };

        var repo = new FakeMovieRepository();
        repo.Seed(movie);

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new UpdateMovieCommandHandler(uow);

        var command = new UpdateMovieCommand(
            Id: movie.Id,
            Title: "New",
            Description: "Updated",
            DurationMinutes: 110,
            Genre: "Action",
            Rating: "PG",
            PosterUrl: "http://example.com/new.jpg",
            ReleaseDate: DateTime.UtcNow.AddDays(20),
            IsActive: false
        );

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ExistingMovie_PropertiesUpdatedCorrectly()
    {
        // ARRANGE
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = "Old Title",
            Description = "Old Desc",
            DurationMinutes = 90,
            Genre = "Drama",
            Rating = "R",
            PosterUrl = "http://example.com/old.png",
            ReleaseDate = new DateTime(2025, 1, 1),
            IsActive = true
        };

        var repo = new FakeMovieRepository();
        repo.Seed(movie);

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new UpdateMovieCommandHandler(uow);

        var command = new UpdateMovieCommand(
            Id: movie.Id,
            Title: "New Title",
            Description: "New Desc",
            DurationMinutes: 120,
            Genre: "Action",
            Rating: "PG-13",
            PosterUrl: "http://example.com/new.png",
            ReleaseDate: new DateTime(2030, 1, 1),
            IsActive: false
        );

        // ACT
        await handler.Handle(command, CancellationToken.None);

        // ASSERT (state verification)
        var updated = await uow.Movies.GetByIdAsync(movie.Id);
        Assert.NotNull(updated);

        Assert.Equal("New Title", updated!.Title);
        Assert.Equal("New Desc", updated.Description);
        Assert.Equal(120, updated.DurationMinutes);
        Assert.Equal("Action", updated.Genre);
        Assert.Equal("PG-13", updated.Rating);
        Assert.Equal("http://example.com/new.png", updated.PosterUrl);
        Assert.Equal(new DateTime(2030, 1, 1), updated.ReleaseDate);
        Assert.False(updated.IsActive);
    }

    [Fact]
    public async Task Handle_NonExistentMovie_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new UpdateMovieCommandHandler(uow);

        var command = new UpdateMovieCommand(
            Id: Guid.NewGuid(),
            Title: "X",
            Description: "X",
            DurationMinutes: 1,
            Genre: "X",
            Rating: "X",
            PosterUrl: "X",
            ReleaseDate: DateTime.UtcNow,
            IsActive: true
        );

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(command, CancellationToken.None));
    }
}
