using System.Text.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;
[ApiController]
[Route("api/map")]
public class MapController:ControllerBase
{
    private readonly AppDbContext _db;
    public MapController(AppDbContext db)
    {
        _db = db;
    }

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
        if (_db.Map.Any(u => u.Name == request.Name))
            return Conflict("Map already exists");

        var newMap = new Map
        {
            Name = request.Name,
            Mode = request.Mode,
            ModeId = request.ModeId
        };
        
        _db.Map.Add(newMap);
        _db.SaveChanges();
        return Ok(JsonSerializer.Serialize(newMap));
    }
}

public class CreateMapRequest
{
    public string Name { get; set; } = "";
    public string Mode { get; set; } = "";
    public int ModeId { get; set; }
}

