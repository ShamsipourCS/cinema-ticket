using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Features.Movies.Commands.CreateMovie;
using CinemaTicket.Application.Tests.Fakes;

namespace CinemaTicket.Application.Tests.Features.Movies;

public class CreateMovieCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewMovieId()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreateMovieCommandHandler(uow);

        var command = new CreateMovieCommand(
            Title: "Test Movie",
            Description: "Test Description",
            DurationMinutes: 120,
            Genre: "Action",
            Rating: "PG-13",
            PosterUrl: "http://example.com/poster.jpg",
            ReleaseDate: DateTime.UtcNow.AddDays(30),
            IsActive: true
        );

        // ACT
        var movieId = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, movieId);

        var fakeRepo = (FakeMovieRepository)uow.Movies;
        Assert.Equal(1, fakeRepo.AddCalls);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ValidCommand_MovieExistsInRepository()
    {
        // ARRANGE
        var movieRepo = new FakeMovieRepository();

        using var uow = new FakeUnitOfWork(
            movieRepo,
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreateMovieCommandHandler(uow);

        var command = new CreateMovieCommand(
            Title: "Expected Title",
            Description: "Expected Description",
            DurationMinutes: 90,
            Genre: "Drama",
            Rating: "R",
            PosterUrl: "http://example.com/x.jpg",
            ReleaseDate: DateTime.UtcNow.AddDays(10),
            IsActive: true
        );

        // ACT
        var movieId = await handler.Handle(command, CancellationToken.None);

        // ASSERT (state verification pattern from guide)
        var created = await uow.Movies.GetByIdAsync(movieId);
        Assert.NotNull(created);
        Assert.Equal("Expected Title", created!.Title);
        Assert.Equal("Drama", created.Genre);
        Assert.True(created.IsActive);
    }
}
