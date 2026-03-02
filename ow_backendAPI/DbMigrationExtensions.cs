using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Data;

namespace ow_backendAPI;

public static class DbMigrationExtensions
{
    public static async Task ApplyMigrationsAsync(this WebApplication services)
    {
        await using var scope = services.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}