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

    /// <summary>
    /// Gets or sets the Movies database set.
    /// </summary>
    public DbSet<Movie> Movies { get; set; }

    /// <summary>
    /// Gets or sets the Cinemas database set.
    /// </summary>
    public DbSet<Cinema> Cinemas { get; set; }

    /// <summary>
    /// Gets or sets the Halls database set.
    /// </summary>
    public DbSet<Hall> Halls { get; set; }

    /// <summary>
    /// Gets or sets the Seats database set.
    /// </summary>
    public DbSet<Seat> Seats { get; set; }

    /// <summary>
    /// Gets or sets the Showtimes database set.
    /// </summary>
    public DbSet<Showtime> Showtimes { get; set; }

    /// <summary>
    /// Gets or sets the Tickets database set.
    /// </summary>
    public DbSet<Ticket> Tickets { get; set; }

    /// <summary>
    /// Gets or sets the Payments database set.
    /// </summary>
    public DbSet<Payment> Payments { get; set; }

    // Repository instances - lazily initialized
    private IUserRepository? _userRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;

    // IUnitOfWork Repository Properties
    IUserRepository IUnitOfWork.Users => _userRepository ??= new UserRepository(this);
    IRefreshTokenRepository IUnitOfWork.RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(this);

    public object Tickets { get; internal set; }

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
        base.OnModelCreating(modelBuilder);

        // پیکربندی روابط
        modelBuilder.Entity<Ticket>()
        .HasOne(t => t.Showtime)
        .WithMany(s => s.Tickets)
        .HasForeignKey(t => t.ShowtimeId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Seat)
            .WithMany()
            .HasForeignKey(t => t.SeatId)
            .OnDelete(DeleteBehavior.Restrict);

        // محدودیت یکتا برای TicketNumber
        modelBuilder.Entity<Ticket>()
            .HasIndex(t => t.TicketNumber)
            .IsUnique();

        // تنظیم فیلدهایی که نباید نال باشند
        modelBuilder.Entity<Ticket>()
            .Property(t => t.HolderName)
            .IsRequired();  // این خط باعث می‌شود که HolderName الزامی باشد

        modelBuilder.Entity<Ticket>()
            .Property(t => t.Price)
            .IsRequired();  // مثال دیگر از استفاده از IsRequired برای فیلدهای عددی
    }

}
