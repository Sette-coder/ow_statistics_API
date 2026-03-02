using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ow_backendAPI;
using ow_backendAPI.Data;

var builder = WebApplication.CreateBuilder(args);

if(builder.Environment.IsProduction())
{
    Env.Load(".production.env");
}

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// Add environment variables to config
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.GetSection("Postgres");

var connString = new NpgsqlConnectionStringBuilder
{
    Host = builder.Configuration["Database:Host"],
    Username = builder.Configuration["Database:Username"],
    Password = builder.Configuration["Database:Password"],
    Database = builder.Configuration["Database:Name"],
    Port = 5432
}.ToString();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null );

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connString, npgSqlBuilder =>
    {
        npgSqlBuilder.MigrationsHistoryTable("migrations", schema: "data");
    });
});

var app = builder.Build();

await app.ApplyMigrationsAsync();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseCors();
app.MapControllers();
app.MapGet("/health", () => Results.Ok("ok"));
app.Run();