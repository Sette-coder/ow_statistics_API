using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Models;

namespace ow_backendAPI.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options){}
    
    public DbSet<AppUser>      AppUsers      { get; set; } = null!;
    public DbSet<Map>          Maps          { get; set; } = null!;
    public DbSet<Hero>         Heroes        { get; set; } = null!;
    public DbSet<Match>        Matches       { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("data");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}