using System.Text;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using ow_backendAPI;
using ow_backendAPI.Data;
using ow_backendAPI.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ─── Environment ───────────────────────────────────────────
Env.Load(builder.Environment.IsProduction() ? ".production.env" : ".local.env");

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000);
});

// ─── Configuration ─────────────────────────────────────────
builder.Configuration.AddEnvironmentVariables();

// ─── JWT ───────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ClockSkew = TimeSpan.Zero
    };
});

// ─── Database ──────────────────────────────────────────────
var connString = new NpgsqlConnectionStringBuilder
{
    Host = builder.Configuration["Database:Host"],
    Username = builder.Configuration["Database:Username"],
    Password = builder.Configuration["Database:Password"],
    Database = builder.Configuration["Database:Name"],
    Port = 5432
}.ToString();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(connString, npgsql =>
    {
        npgsql.MigrationsHistoryTable("migrations", schema: "data");
    });
});

// ─── Repositories ──────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// ─── CORS ──────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ─── General ───────────────────────────────────────────────
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.PropertyNamingPolicy = null);

// ─── Pipeline ──────────────────────────────────────────────
var app = builder.Build();

await app.ApplyMigrationsAsync();
await AdminSeeder.SeedAdminAsync(app.Services, builder.Configuration);
await StaticDataSeeder.SeedAsync(app.Services);

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();            // must be before UseAuthentication
app.UseAuthentication();  // ← validates the JWT token
app.UseAuthorization();   // ← checks [Authorize] attributes
app.MapControllers();
app.MapGet("/health", () => Results.Ok("ok"));

app.Run();