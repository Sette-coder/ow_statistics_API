using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;
[ApiController]
[Route("owstatistics/api/hero")]
public class HeroController(AppDbContext db) : ControllerBase
{
    [HttpPost("create")]
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
        if (db.Hero.Any(u => u.Name == request.Name))
            return Conflict("Hero already exists");

        var newHero = new Hero
        {
            Name = request.Name,
            Role = request.Role,
        };
        
        db.Hero.Add(newHero);
        db.SaveChanges();
        return Ok(JsonSerializer.Serialize(newHero));
    }
}

public class CreateHeroRequest
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}

