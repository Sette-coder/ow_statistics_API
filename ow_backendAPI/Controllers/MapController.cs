using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;
[ApiController]
[Route("owstatistics/api/map")]
public class MapController(AppDbContext db) : ControllerBase
{
    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateMapRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Map Name is Missing");
        }
        if (string.IsNullOrWhiteSpace(request.Mode) && request.Mode == "Default")
        {
            return BadRequest("Map Mode cannot be Default");
        }
        if (db.Map.Any(u => u.Name == request.Name))
            return Conflict("Map already exists");

        var newMap = new Map
        {
            Name = request.Name,
            Mode = request.Mode,
            ModeId = request.ModeId
        };
        
        db.Map.Add(newMap);
        db.SaveChanges();
        return Ok(newMap);
    }
    
    [HttpGet("get-all-maps")]
    public async Task<IActionResult> GetAllMaps()
    {
        var response = await db.Map
            .Select(m => new FromDatabaseMaps
            {
                Id = m.Id,
                Name = m.Name,
                Mode = m.Mode,
                ModeId = m.ModeId
            })
            .ToListAsync();
        return Ok(response);
    }
}

public class CreateMapRequest
{
    public string Name { get; set; } = "";
    public string Mode { get; set; } = "";
    public int ModeId { get; set; }
}

public class FromDatabaseMaps
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Mode { get; set; }
    public int ModeId { get; set; }
}

public class FromDatabaseMapsResponse()
{
    public List<FromDatabaseMaps> Maps { get; set; }
}

