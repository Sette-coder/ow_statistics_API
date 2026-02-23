using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;
[ApiController]
[Route("owstatistics/api/hero")]
public class HeroController:ControllerBase
{
    private readonly AppDbContext _db;
    public HeroController(AppDbContext db)
    {
        _db = db;
    }

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
        if (_db.Hero.Any(u => u.Name == request.Name))
            return Conflict("Hero already exists");

        var newHero = new Hero
        {
            Name = request.Name,
            Role = request.Role,
        };
        
        _db.Hero.Add(newHero);
        _db.SaveChanges();
        return Ok(JsonSerializer.Serialize(newHero));
    }
}

public class CreateHeroRequest
{
    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
}

