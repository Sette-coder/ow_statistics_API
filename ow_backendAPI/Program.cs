using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ow_backendAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Load .local.env only for Development
if (builder.Environment.IsDevelopment())
{
    Env.Load(".local.env");
}else if(builder.Environment.IsProduction())
{
    Env.Load(".production.env");
}

// Add environment variables to config
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.GetSection("Postgres");

var connString = new NpgsqlConnectionStringBuilder
{
    Host = builder.Configuration["Database:Host"],
    Username = builder.Configuration["Database:Username"],
    Password = builder.Configuration["Database:Password"],
    Database = builder.Configuration["Database:Name"],
    Port = 5432,
    SslMode = SslMode.VerifyFull,
    RootCertificate = builder.Configuration["Database:CertificateDir"],
}.ToString();
Console.WriteLine($"Connecting to database: {connString}");
NpgsqlConnection conn = new NpgsqlConnection(connString);
try {
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