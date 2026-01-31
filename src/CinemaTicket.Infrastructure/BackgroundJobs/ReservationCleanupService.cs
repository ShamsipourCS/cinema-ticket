using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Persistence.Context;
using CinemaTicket.Domain.Enums;

namespace CinemaTicket.Infrastructure.BackgroundJobs;

public sealed class ReservationCleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReservationCleanupService> _logger;

    private static readonly TimeSpan RunEvery = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan PendingTtl = TimeSpan.FromMinutes(10);

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
        var stripeService = scope.ServiceProvider.GetRequiredService<IStripePaymentService>();

        var cutoff = DateTime.UtcNow - PendingTtl;

        // Find expired pending tickets with their associated payments
        var oldPendingTickets = await db.Tickets
            .Include(t => t.Payment)
            .Where(t => t.Status == TicketStatus.Pending && t.CreatedAt <= cutoff)
            .ToListAsync(ct);

        if (oldPendingTickets.Count == 0)
            return;

        var canceledPaymentCount = 0;
        var failedPaymentCancellations = new List<string>();

        // Process each expired ticket
        foreach (var ticket in oldPendingTickets)
        {
            // Update ticket status to Expired
            ticket.Status = TicketStatus.Expired;

            // If ticket has an associated payment, attempt to cancel the payment intent
            if (ticket.Payment is not null && ticket.Payment.Status == PaymentStatus.Pending)
            {
                try
                {
                    var canceled = await stripeService.CancelPaymentIntentAsync(
                        ticket.Payment.StripePaymentIntentId,
                        ct);

                    if (canceled)
                    {
                        // Update payment status to Refunded
                        ticket.Payment.Status = PaymentStatus.Refunded;
                        canceledPaymentCount++;

                        _logger.LogDebug(
                            "Canceled payment intent {PaymentIntentId} for expired ticket {TicketId}",
                            ticket.Payment.StripePaymentIntentId, ticket.Id);
                    }
                    else
                    {
                        _logger.LogWarning(
                            "Failed to cancel payment intent {PaymentIntentId} for ticket {TicketId} - may already be succeeded",
                            ticket.Payment.StripePaymentIntentId, ticket.Id);
                        failedPaymentCancellations.Add(ticket.Payment.StripePaymentIntentId);
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue processing other tickets
                    _logger.LogError(ex,
                        "Error canceling payment intent {PaymentIntentId} for ticket {TicketId}",
                        ticket.Payment.StripePaymentIntentId, ticket.Id);
                    failedPaymentCancellations.Add(ticket.Payment.StripePaymentIntentId);
                }
            }
        }

        // Save all changes (ticket status updates and payment status updates)
        await db.SaveChangesAsync(ct);

        // Log summary
        _logger.LogInformation(
            "ReservationCleanupService: Expired {TicketCount} pending tickets, " +
            "canceled {PaymentCount} payment intents, {FailedCount} payment cancellations failed",
            oldPendingTickets.Count, canceledPaymentCount, failedPaymentCancellations.Count);

        if (failedPaymentCancellations.Count > 0)
        {
            _logger.LogWarning(
                "Failed to cancel payment intents: {PaymentIntentIds}",
                string.Join(", ", failedPaymentCancellations));
        }
    }
}
