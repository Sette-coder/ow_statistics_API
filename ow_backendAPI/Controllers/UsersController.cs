using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ow_backendAPI.Models;
using ow_backendAPI.Repositories;
using ow_backendAPI.Services;

namespace ow_backendAPI.Controllers;

[ApiController]
[Route("owstatistics/api/user")]
public class UsersController(
    IUserRepository userRepo,
    IRefreshTokenRepository tokenRepo,
    JwtService jwtService,
    IConfiguration config) : ControllerBase
{
    private readonly PasswordHasher<AppUser> _passwordHasher = new();

    // ─── Create ────────────────────────────────────────────
    [HttpPost("create")]
    public async Task<IActionResult> CreateAsync([FromBody] CreateUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest(new GenericResponse { ResponseMessage = "Username missing" });

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new GenericResponse { ResponseMessage = "Email missing" });

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new GenericResponse { ResponseMessage = "Password missing" });

        if (await userRepo.ExistsByEmailAsync(request.Email))
            return Conflict(new GenericResponse { ResponseMessage = "Email already in use" });

        if (await userRepo.ExistsAsync(request.Username))
            return Conflict(new GenericResponse { ResponseMessage = "Username already in use" });

        var user = new AppUser
        {
            Username = request.Username,
            Email = request.Email,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Client" : request.Role,
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await userRepo.CreateAsync(user);

        return Ok(new GenericResponse { Ok = true, ResponseMessage = "User created successfully" });
    }

    // ─── Login ─────────────────────────────────────────────
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UsernameOrEmail) ||
            string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new GenericResponse { ResponseMessage = "Credentials missing" });

        var user = await userRepo.GetByUsernameOrEmailAsync(request.UsernameOrEmail);

        // Return same message for both "not found" and "wrong password" — avoids user enumeration
        if (user == null || _passwordHasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return Unauthorized(new GenericResponse { ResponseMessage = "Invalid credentials" });

        // Revoke any previous sessions for this user
        await tokenRepo.RevokeAllUserTokensAsync(user.Id);

        var accessToken = jwtService.GenerateAccessToken(user.Id, user.Username, user.Role);
        var refreshToken = jwtService.GenerateRefreshToken();

        await tokenRepo.SaveRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(
                double.Parse(config["JwtSettings:RefreshTokenExpirationDays"]!))
        });

        return Ok(new LoginResponse
        {
            Authorized = true,
            UserId = user.Id,
            Username = user.Username,
            UserEmail = user.Email,
            Role = user.Role,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            LoginMessage = "Logged in successfully"
        });
    }

    // ─── Refresh ───────────────────────────────────────────
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(new GenericResponse { ResponseMessage = "Refresh token missing" });

        var stored = await tokenRepo.GetValidTokenAsync(request.RefreshToken);
        if (stored == null)
            return Unauthorized(new GenericResponse { ResponseMessage = "Session expired, please log in again" });

        var user = await userRepo.GetByIdAsync(stored.UserId);
        if (user == null)
            return Unauthorized(new GenericResponse { ResponseMessage = "User not found" });

        // Rotate tokens — revoke old, issue new pair
        await tokenRepo.RevokeTokenAsync(request.RefreshToken);

        var newAccessToken = jwtService.GenerateAccessToken(user.Id, user.Username, user.Role);
        var newRefreshToken = jwtService.GenerateRefreshToken();

        await tokenRepo.SaveRefreshTokenAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(
                double.Parse(config["JwtSettings:RefreshTokenExpirationDays"]!))
        });

        return Ok(new RefreshResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    // ─── Logout ────────────────────────────────────────────
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
            return BadRequest(new GenericResponse { ResponseMessage = "Refresh token missing" });

        await tokenRepo.RevokeTokenAsync(request.RefreshToken);
        return Ok(new GenericResponse { Ok = true, ResponseMessage = "Logged out successfully" });
    }

    // ─── Get own profile ───────────────────────────────────
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized(new GenericResponse { ResponseMessage = "Invalid token" });

        var user = await userRepo.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new GenericResponse { ResponseMessage = "User not found" });

        return Ok(new UserProfileResponse
        {
            UserId = user.Id,
            Username = user.Username,
            UserEmail = user.Email,
            Role = user.Role
        });
    }

    [HttpPut("update/{targetUserId:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int targetUserId, [FromBody] UpdateUserRequest request)
    {
        // Extract caller identity from JWT
        var callerIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (callerIdClaim == null || !int.TryParse(callerIdClaim, out var callerId))
            return Unauthorized(new GenericResponse { ResponseMessage = "Invalid token" });

        var callerRole = User.FindFirst(ClaimTypes.Role)?.Value;
        var isAdmin = callerRole == "Admin";
        var isOwner = callerId == targetUserId;

        // Only the owner or an Admin can modify this user
        if (!isOwner && !isAdmin)
            return StatusCode(403, new GenericResponse { ResponseMessage = "You can only modify your own account" });

        var target = await userRepo.GetByIdAsync(targetUserId);
        if (target == null)
            return NotFound(new GenericResponse { ResponseMessage = "User not found" });

        // ── Username change ────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(request.Username) &&
            request.Username != target.Username)
        {
            if (await userRepo.ExistsAsync(request.Username))
                return Conflict(new GenericResponse { ResponseMessage = "Username already in use" });

            target.Username = request.Username;
        }

        // ── Email change ───────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(request.Email) &&
            request.Email != target.Email)
        {
            if (await userRepo.ExistsByEmailAsync(request.Email))
                return Conflict(new GenericResponse { ResponseMessage = "Email already in use" });

            target.Email = request.Email;
        }

        // ── Password change ────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(request.NewPassword))
        {
            // Non-admins must confirm their current password to change it
            if (!isAdmin)
            {
                if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                    return BadRequest(new GenericResponse
                        { ResponseMessage = "Current password required to set a new one" });

                var verification = _passwordHasher.VerifyHashedPassword(
                    target, target.PasswordHash, request.CurrentPassword);

                if (verification == PasswordVerificationResult.Failed)
                    return Unauthorized(new GenericResponse { ResponseMessage = "Current password is incorrect" });
            }

            target.PasswordHash = _passwordHasher.HashPassword(target, request.NewPassword);

            // Invalidate all existing sessions — forces re-login with new password
            await tokenRepo.RevokeAllUserTokensAsync(target.Id);
        }

        // ── Role change — Admin only ───────────────────────────
        if (!string.IsNullOrWhiteSpace(request.Role))
        {
            if (!isAdmin)
                return StatusCode(403, new GenericResponse { ResponseMessage = "Only admins can change roles" });

            target.Role = request.Role;
        }

        await userRepo.UpdateAsync(target);

        return Ok(new UpdateUserResponse
        {
            Ok = true,
            ResponseMessage = "User updated successfully",
            UserId = target.Id,
            Username = target.Username,
            UserEmail = target.Email,
            Role = target.Role,
            SessionsRevoked = !string.IsNullOrWhiteSpace(request.NewPassword)
        });
    }
}

// ─── Requests ──────────────────────────────────────────────
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

public class RefreshRequest
{
    public string RefreshToken { get; set; } = "";
}

// ─── Responses ─────────────────────────────────────────────
public class GenericResponse
{
    public bool Ok { get; set; } = false;
    public string ResponseMessage { get; set; } = "";
}

public class LoginResponse
{
    public bool Authorized { get; set; } = false;
    public int UserId { get; set; } = -1;
    public string Username { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string Role { get; set; } = "";
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
    public string LoginMessage { get; set; } = "";
}

public class RefreshResponse
{
    public string AccessToken { get; set; } = "";
    public string RefreshToken { get; set; } = "";
}

public class UserProfileResponse
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string Role { get; set; } = "";
}

public class UpdateUserRequest
{
    public string? Username { get; set; } // null = no change
    public string? Email { get; set; } // null = no change
    public string? CurrentPassword { get; set; } // required only when changing password as non-admin
    public string? NewPassword { get; set; } // null = no change
    public string? Role { get; set; } // null = no change, Admin only
}

public class UpdateUserResponse
{
    public bool Ok { get; set; } = false;
    public string ResponseMessage { get; set; } = "";
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string Role { get; set; } = "";
    public bool SessionsRevoked { get; set; } = false; // tells Unity to force re-login
}