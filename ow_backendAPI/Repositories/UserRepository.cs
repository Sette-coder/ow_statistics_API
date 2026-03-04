using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Repositories;

public interface IUserRepository
{
    /// <summary>Returns a user by username, or null if not found.</summary>
    Task<AppUser?> GetByUsernameAsync(string username);

    /// <summary>Returns a user by id, or null if not found.</summary>
    Task<AppUser?> GetByIdAsync(int id);

    /// <summary>Returns true if a user with the given username already exists.</summary>
    Task<bool> ExistsAsync(string username);
    
    Task<bool> ExistsByEmailAsync(string email);
    Task<AppUser?> GetByUsernameOrEmailAsync(string usernameOrEmail);

    /// <summary>Persists a new user to the database.</summary>
    Task CreateAsync(AppUser user);
    
    Task UpdateAsync(AppUser user);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<AppUser?> GetByUsernameAsync(string username) =>
        await _db.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);

    public async Task<AppUser?> GetByIdAsync(int id) =>
        await _db.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<bool> ExistsAsync(string username) =>
        await _db.AppUsers
            .AnyAsync(u => u.Username == username);
    
    public async Task<bool> ExistsByEmailAsync(string email) =>
        await _db.AppUsers.AnyAsync(u => u.Email == email);

    public async Task<AppUser?> GetByUsernameOrEmailAsync(string usernameOrEmail) =>
        await _db.AppUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u =>
                u.Username == usernameOrEmail ||
                u.Email == usernameOrEmail);
    
    public async Task UpdateAsync(AppUser user)
    {
        _db.AppUsers.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task CreateAsync(AppUser user)
    {
        _db.AppUsers.Add(user);
        await _db.SaveChangesAsync();
    }
}