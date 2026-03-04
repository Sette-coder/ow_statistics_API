using Microsoft.AspNetCore.Identity;
using ow_backendAPI.Models;
using ow_backendAPI.Repositories;

namespace ow_backendAPI.Data;

public static class AdminSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();

        var userRepo      = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var passwordHasher = new PasswordHasher<AppUser>();

        var username = config["Admin:Username"];
        var email    = config["Admin:Email"];
        var password = config["Admin:Password"];

        // Validate env vars are set
        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(email)    ||
            string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("[AdminSeeder] ⚠️  Admin credentials not set in environment — skipping.");
            return;
        }

        // Skip if admin already exists
        if (await userRepo.ExistsAsync(username))
        {
            Console.WriteLine("[AdminSeeder] ✅ Admin user already exists — skipping.");
            return;
        }

        var admin = new AppUser
        {
            Username = username,
            Email    = email,
            Role     = "Admin",
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, password);

        await userRepo.CreateAsync(admin);
        Console.WriteLine($"[AdminSeeder] ✅ Admin user '{username}' created successfully.");
    }
}