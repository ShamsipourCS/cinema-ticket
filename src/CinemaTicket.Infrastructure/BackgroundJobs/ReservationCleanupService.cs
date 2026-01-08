using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CinemaTicket.Persistence.Context;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Infrastructure.BackgroundJobs;

public sealed class ReservationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationCleanupService> _logger;

    private static readonly TimeSpan RunEvery = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan PendingTtl = TimeSpan.FromMinutes(15);

    public ReservationCleanupService(IServiceScopeFactory scopeFactory, ILogger<ReservationCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(RunEvery);

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await CleanupOnce(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Reservation cleanup job failed.");
            }
        }
    }

    private async Task CleanupOnce(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var cutoff = DateTime.UtcNow - PendingTtl;

        var oldPendingTickets = await db.Tickets
            .Where(t => t.Status == TicketStatus.Pending && t.CreatedAt <= cutoff)
            .ToListAsync(ct);

        if (oldPendingTickets.Count == 0)
            return;

        foreach (var t in oldPendingTickets)
            t.Status = TicketStatus.Cancelled;

        await db.SaveChangesAsync(ct);

        _logger.LogInformation("ReservationCleanupService cancelled {Count} old pending tickets.", oldPendingTickets.Count);
    }
}
