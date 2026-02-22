using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ow_backendAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Load .local.env only for Development
if (builder.Environment.IsDevelopment())
{
    Env.Load(".local.env");
}

// Add environment variables to config
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.GetSection("Postgres");

var dbHost = builder.Configuration["Database:Host"];
var dbPassword = builder.Configuration["Database:Password"];
var dbName = builder.Configuration["Database:Name"];
var dbUsername = builder.Configuration["Database:Username"];
string connString = $"Host={dbHost};Port=5432;Database={dbName};Username={dbUsername};Password={dbPassword};SSL Mode=VerifyFull;Root Certificate=./certs/global-bundle.pem";

NpgsqlConnection conn = null;
try {
    conn = new NpgsqlConnection(connString);
    conn.Open();
    using var cmd = new NpgsqlCommand("SELECT version();", conn);
    Console.WriteLine(cmd.ExecuteScalar());
} catch (Exception ex) {
    Console.WriteLine($"Database error: {ex.Message}");
    throw;
} finally {
    conn?.Close();
    conn?.Dispose();
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connString);
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();
app.MapControllers();
app.Run();