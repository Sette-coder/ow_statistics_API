using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Controllers;
using ow_backendAPI.Models;

namespace ow_backendAPI.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options){}
    
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Map> Map { get; set; }
    public DbSet<Hero> Hero { get; set; }
    public DbSet<Match> Match => Set<Match>();
    public DbSet<RefreshToken> RefreshTokens { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("data");
        modelBuilder.ApplyConfiguration(new MatchEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AppUserEntityConfiguration());
    }
}