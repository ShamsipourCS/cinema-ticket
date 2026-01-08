using CinemaTicket.Application.Features.Movies.Commands.DeleteMovie;
using CinemaTicket.Application.Tests.Fakes;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Application.Tests.Features.Movies;

public class DeleteMovieCommandHandlerTests
{
    [Fact]
    public async Task Handle_ExistingMovie_SavesChanges()
    {
        // ARRANGE
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = "To Delete",
            Genre = "Drama",
            IsActive = true
        };

        var repo = new FakeMovieRepository();
        repo.Seed(movie);

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new DeleteMovieCommandHandler(uow);

        // ACT
        await handler.Handle(new DeleteMovieCommand(movie.Id), CancellationToken.None);

        // ASSERT
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ExistingMovie_RemovesMovieFromRepository()
    {
        // ARRANGE
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = "To Delete",
            Genre = "Drama",
            IsActive = true
        };

        var repo = new FakeMovieRepository();
        repo.Seed(movie);

        using var uow = new FakeUnitOfWork(repo, new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new DeleteMovieCommandHandler(uow);

        // ACT
        await handler.Handle(new DeleteMovieCommand(movie.Id), CancellationToken.None);

        // ASSERT (state verification)
        var deleted = await uow.Movies.GetByIdAsync(movie.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task Handle_NonExistentMovie_ThrowsKeyNotFoundException()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new DeleteMovieCommandHandler(uow);

        // ACT + ASSERT
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new DeleteMovieCommand(Guid.NewGuid()), CancellationToken.None));
    }
}
