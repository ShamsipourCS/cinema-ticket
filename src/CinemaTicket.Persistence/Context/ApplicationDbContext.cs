using CinemaTicket.Application.Common.Interfaces;
using CinemaTicket.Domain.Common;
using CinemaTicket.Domain.Entities;
using CinemaTicket.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Context;

/// <summary>
/// Entity Framework Core database context for the cinema ticket booking system.
/// Implements the IUnitOfWork interface for managing database transactions.
/// </summary>
public class ApplicationDbContext : DbContext, IUnitOfWork
{
    /// <summary>
    /// Initializes a new instance of the ApplicationDbContext class.
    /// </summary>
    /// <param name="options">The database context options.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the Users database set.
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Gets or sets the RefreshTokens database set.
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    // Repository instances - lazily initialized
    private IUserRepository? _userRepository;

    // IUnitOfWork Repository Properties
    IUserRepository IUnitOfWork.Users => _userRepository ??= new UserRepository(this);

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.BeginTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
    }
    
    /// <inheritdoc />
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
