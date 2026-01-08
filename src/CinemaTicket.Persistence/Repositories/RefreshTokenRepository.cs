using CinemaTicket.Domain.Entities;
using CinemaTicket.Domain.Interfaces;
using CinemaTicket.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace CinemaTicket.Persistence.Repositories;

/// <summary>
/// Implementation of the IRefreshTokenRepository for EF Core.
/// </summary>
public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
{
    /// <summary>
    /// Initializes a new instance of the RefreshTokenRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public RefreshTokenRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string token, Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<RefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(rt =>
                rt.Token == token &&
                rt.UserId == userId &&
                !rt.IsRevoked &&
                rt.ExpiresAt > now,
                cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.Set<RefreshToken>()
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.Token == token, cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        // Note: Caller must call SaveChangesAsync on IUnitOfWork
    }

    public async Task<int> RemoveExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var expiredTokens = await _context.Set<RefreshToken>()
            .Where(rt => rt.ExpiresAt <= now)
            .ToListAsync(cancellationToken);

        _context.Set<RefreshToken>().RemoveRange(expiredTokens);

        // Note: Caller must call SaveChangesAsync on IUnitOfWork
        return expiredTokens.Count;
    }

    public async Task<IEnumerable<RefreshToken>> GetActiveTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Set<RefreshToken>()
            .AsNoTracking()
            .Where(rt =>
                rt.UserId == userId &&
                !rt.IsRevoked &&
                rt.ExpiresAt > now)
            .ToListAsync(cancellationToken);
    }
}
