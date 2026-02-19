using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;

[ApiController]
[Route("api/user")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<AppUser> _passwordHasher;

    public UsersController(AppDbContext db)
    {
        _db = db;
        _passwordHasher = new PasswordHasher<AppUser>();
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            return BadRequest("Username Missing");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Password Missing");
        }

        if (_db.AppUsers.Any(u => u.Email == request.Email))
            return Conflict("User With this email already exists");

        if (_db.AppUsers.Any(u => u.Username == request.Username))
            return Conflict("User With this username already exists");

        var user = new AppUser
        {
            Username = request.Username,
            Email = request.Email,
        };

        user.Password = _passwordHasher.HashPassword(user, request.Password);

        _db.AppUsers.Add(user);
        _db.SaveChanges();
        var response = new { user.Id, user.Username, user.Email };
        return Ok(JsonSerializer.Serialize(response));
    }

    // LOGIN EXAMPLE
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        LoginResponse response = new LoginResponse
        {
            Authorized = false,
            LoginMessage = ""
        };

        var user = _db.AppUsers.FirstOrDefault(u => u.Username == request.UsernameOrEmail);
        
        if (user == null)
        {
            user = _db.AppUsers.FirstOrDefault(u => u.Email == request.UsernameOrEmail);
            if (user == null)
            {
                Console.WriteLine("USER NOT FOUND ERROR");
                response.LoginMessage = "User not found";
                return Unauthorized(JsonSerializer.Serialize(response));
            }
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            response.LoginMessage = "Invalid Password";
            return Unauthorized(JsonSerializer.Serialize(response));
        }
        
        response.Authorized = true;
        response.Username = user.Username;
        response.UserEmail = user.Email;
        response.LoginMessage = "Logged in!";
        return Ok(JsonSerializer.Serialize(response));
    }
}

public class CreateUserRequest
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginRequest
{
    public string UsernameOrEmail { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public bool Authorized { get; set; } = false;
    public string Username { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string LoginMessage { get; set; } = "";
}