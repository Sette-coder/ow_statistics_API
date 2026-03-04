using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Repositories;

public interface IRefreshTokenRepository
{
    /// <summary>Persists a new refresh token to the database.</summary>
    Task SaveRefreshTokenAsync(RefreshToken token);

    /// <summary>Returns a valid (non-revoked, non-expired) token, or null.</summary>
    Task<RefreshToken?> GetValidTokenAsync(string token);

    /// <summary>Revokes a single refresh token by its value.</summary>
    Task RevokeTokenAsync(string token);

    /// <summary>Revokes all active refresh tokens for a given user.</summary>
    Task RevokeAllUserTokensAsync(int userId);
}

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _db;

    public RefreshTokenRepository(AppDbContext db) => _db = db;

    public async Task SaveRefreshTokenAsync(RefreshToken token)
    {
        _db.RefreshTokens.Add(token);
        await _db.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string token) =>
        await _db.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt =>
                rt.Token == token &&
                rt.RevokedAt == null &&
                rt.ExpiresAt > DateTime.UtcNow);

    public async Task RevokeTokenAsync(string token)
    {
        var rt = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == token);

        if (rt is null) return;

        rt.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        var activeTokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
            .ToListAsync();

        if (!activeTokens.Any()) return;

        foreach (var rt in activeTokens)
            rt.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
    }
}