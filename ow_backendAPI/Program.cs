using Microsoft.EntityFrameworkCore;
using Npgsql;
using ow_backendAPI.Data;

var builder = WebApplication.CreateBuilder(args);

NpgsqlConnection conn = null;
try {
    conn = new NpgsqlConnection(builder.Configuration.GetConnectionString("Postgres"));
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors();
app.MapControllers();
app.Run();