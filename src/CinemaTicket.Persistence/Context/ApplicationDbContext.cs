using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Common;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Domain.Entities;

namespace CinemaTicket.Persistence.Context;

public class ApplicationDbContext : DbContext, IUnitOfWork
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Cinema> Cinemas => Set<Cinema>();
    public DbSet<Hall> Halls => Set<Hall>();
    public DbSet<Seat> Seats => Set<Seat>();


    public async Task BeginTransactionAsync()
    {
        await Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        await Database.CommitTransactionAsync();
    }

    public async Task RollbackTransactionAsync()
    {
        await Database.RollbackTransactionAsync();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries<AuditableEntity>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            // If we had LastModifiedAt, we would handle EntityState.Modified here
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This automatically applies all IEntityTypeConfiguration<T> configurations 
        // found in this assembly (CinemaTicket.Persistence)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
