using CinemaTicket.Application.Features.Cinemas.Commands.CreateCinema;
using CinemaTicket.Application.Tests.Fakes;

namespace CinemaTicket.Application.Tests.Features.Cinemas;

public class CreateCinemaCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsNewCinemaId()
    {
        // ARRANGE
        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            new FakeCinemaRepository(),
            new FakeHallRepository());

        var handler = new CreateCinemaCommandHandler(uow);

        var command = new CreateCinemaCommand(
            Name: "Cinema One",
            Address: "Main Street 1",
            City: "Brussels",
            Phone: "123456789",
            IsActive: true
        );

        // ACT
        var cinemaId = await handler.Handle(command, CancellationToken.None);

        // ASSERT
        Assert.NotEqual(Guid.Empty, cinemaId);

        var cinemaRepo = (FakeCinemaRepository)uow.Cinemas;
        Assert.Equal(1, cinemaRepo.AddCalls);
        Assert.Equal(1, uow.SaveChangesCalls);
    }

    [Fact]
    public async Task Handle_ValidCommand_CinemaExistsInRepository()
    {
        // ARRANGE
        var cinemaRepo = new FakeCinemaRepository();

        using var uow = new FakeUnitOfWork(
            new FakeMovieRepository(),
            cinemaRepo,
            new FakeHallRepository());

        var handler = new CreateCinemaCommandHandler(uow);

        var command = new CreateCinemaCommand(
            Name: "Expected Cinema",
            Address: "Some Address",
            City: "Antwerp",
            Phone: "987654321",
            IsActive: true
        );

        // ACT
        var cinemaId = await handler.Handle(command, CancellationToken.None);

        // ASSERT (state verification)
        var created = await uow.Cinemas.GetByIdAsync(cinemaId, CancellationToken.None);
        Assert.NotNull(created);

        Assert.Equal("Expected Cinema", created!.Name);
        Assert.Equal("Antwerp", created.City);
        Assert.True(created.IsActive);
    }
}
