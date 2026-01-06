using System;
using System.Threading;
using System.Threading.Tasks;
using CinemaTicket.Application.Features.Cinemas.Queries.GetCinemaById;
using CinemaTicket.Application.Tests.Fakes;
using Xunit;

namespace CinemaTicket.Application.Tests.Features.Cinemas;

public class GetCinemaByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WhenMissing_ThrowsKeyNotFoundException()
    {
        using var uow = new FakeUnitOfWork(new FakeMovieRepository(), new FakeCinemaRepository(), new FakeHallRepository());
        var handler = new GetCinemaByIdQueryHandler(uow);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            handler.Handle(new GetCinemaByIdQuery(Guid.NewGuid()), CancellationToken.None));
    }
}
