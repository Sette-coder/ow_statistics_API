using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Models;

namespace ow_backendAPI.Data;

public class AppDbContext:DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options){}
    
    public DbSet<AppUser> AppUsers => Set<AppUser>();
    public DbSet<Map> Map => Set<Map>();
    public DbSet<Hero> Hero => Set<Hero>();
    public DbSet<Match> Match => Set<Match>();
}