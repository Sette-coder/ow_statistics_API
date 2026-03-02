using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;

[ApiController]
[Route("owstatistics/api/user")]
public class UsersController(AppDbContext db) : ControllerBase
{
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateUserRequest request)
    {
        GenericResponse response = new GenericResponse { ok = false, ResponseMessage = "" };

        if (string.IsNullOrWhiteSpace(request.Username))
        {
            response.ResponseMessage = "Username Missing";
            return BadRequest(response);
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            response.ResponseMessage = "Password Missing";
            return BadRequest(response);
        }

        if (db.AppUsers.Any(u => u.Email == request.Email))
        {
            response.ResponseMessage = "User With this email already exists";
            return Conflict(response);
        }

        if (db.AppUsers.Any(u => u.Username == request.Username))
        {
            response.ResponseMessage = "User With this username already exists";
            return Conflict(response);
        }

        var user = new AppUser
        {
            Username = request.Username,
            Email = request.Email,
            Role = string.IsNullOrEmpty(request.Role) ? "Client" : request.Role,
        };

        user.Password = _passwordHasher.HashPassword(user, request.Password);

        db.AppUsers.Add(user);
        db.SaveChanges();
        response.ResponseMessage = "User Create Successfully";
        response.ok = true;
        return Ok(response);
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        LoginResponse response = new LoginResponse
        {
            Authorized = false,
            LoginMessage = ""
        };

        var user = db.AppUsers.FirstOrDefault(u =>
            u.Username == request.UsernameOrEmail || 
            u.Email == request.UsernameOrEmail);

        if (user == null)
        {
            Console.WriteLine("USER NOT FOUND ERROR");
            response.LoginMessage = "User not found";
            return Unauthorized(response);
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, request.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            response.LoginMessage = "Invalid Password";
            return Unauthorized(response);
        }

        response.Authorized = true;
        response.UserId = user.Id;
        response.Username = user.Username;
        response.UserEmail = user.Email;
        response.Role = user.Role;
        response.LoginMessage = "Logged in!";

        return Ok(response);
    }
}

public class CreateUserRequest
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "";
}

public class LoginRequest
{
    public string UsernameOrEmail { get; set; } = "";
    public string Password { get; set; } = "";
}

public class LoginResponse
{
    public bool Authorized { get; set; } = false;
    public int UserId { get; set; } = -1;
    public string Username { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string Role { get; set; } = "";
    public string LoginMessage { get; set; } = "";
}

public class GenericResponse
{
    public bool ok { get; set; } = false;
    public string ResponseMessage { get; set; } = "";
}