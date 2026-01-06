using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Features.Movies.Queries.GetMovieById;
using CinemaTicket.Application.Tests.Fakes;
using Xunit;

namespace CinemaTicket.Application.Tests.Features.Movies;

public class GetMovieByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenMissing_ThrowsKeyNotFoundException()
    {
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetMovieByIdQueryHandler(uow);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new GetMovieByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
