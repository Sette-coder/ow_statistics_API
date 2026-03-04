using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;
[ApiController]
[Route("owstatistics/api/hero")]

public class HeroController(AppDbContext db) : ControllerBase
{
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public IActionResult Create([FromBody] CreateHeroRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Map Name is Missing");
        }
        if (string.IsNullOrWhiteSpace(request.Role) && request.Role == "None")
        {
            return BadRequest("Map Mode cannot be None or is Empty");
        }
        if (db.Heroes.Any(u => u.Name == request.Name))
            return Conflict("Hero already exists");

        var newHero = new Hero
        {
            Name = request.Name,
            Role = request.Role,
        };
        
        db.Heroes.Add(newHero);
        db.SaveChanges();
        return Ok(newHero);
    }
    
    [HttpGet("get-all-heroes")]
    [Authorize]
    public async Task<IActionResult> GetAllHeroes()
    {
        var response = await db.Heroes
            .Select(m => new FromDatabaseHeroes
            {
                Id = m.Id,
                Name = m.Name,
                Role = m.Role,
            })
            .ToListAsync();
        
        return Ok(response);
    }
}

public class CreateHeroRequest
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}

public class FromDatabaseHeroes
{
    public int Id { get; set; } = -1;
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}

