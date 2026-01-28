using CinemaTicket.Persistence.Context;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CinemaTicket.Persistence.Seeders;

/// <summary>
/// Master orchestrator for database seeding operations.
/// Coordinates seeding of all entities in correct dependency order.
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Seeds the database with initial data in Development environment.
    /// This method is idempotent and safe to run multiple times.
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="serviceProvider">Service provider for dependency resolution</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public static async Task SeedAsync(
        ApplicationDbContext context,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        var logger = serviceProvider.GetService<ILogger<ApplicationDbContext>>();

        try
        {
            logger?.LogInformation("Starting database seeding process...");

            // Apply pending migrations
            logger?.LogInformation("Applying database migrations...");
            await context.Database.EnsureCreatedAsync(cancellationToken);

            // Phase 1: Independent Entities (no FK dependencies)
            logger?.LogInformation("Phase 1: Seeding independent entities...");
            await UserSeeder.SeedAsync(context, serviceProvider, cancellationToken);
            await MovieSeeder.SeedAsync(context, cancellationToken);

            // Phase 2: Cinema Infrastructure (depends on Phase 1)
            logger?.LogInformation("Phase 2: Seeding cinema infrastructure...");
            await CinemaSeeder.SeedAsync(context, cancellationToken);

            // Phase 3: Scheduling (depends on Phase 1 & 2)
            logger?.LogInformation("Phase 3: Seeding showtimes...");
            await ShowtimeSeeder.SeedAsync(context, cancellationToken);

            // Phase 4: Optional Transactional Data
            logger?.LogInformation("Phase 4: Seeding sample tickets (optional)...");
            await TicketSeeder.SeedAsync(context, cancellationToken);

            logger?.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred during database seeding.");
            throw;
        }
    }
}
