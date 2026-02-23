using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;

[ApiController]
[Route("owstatistics/api/match")]
public class MatchController : ControllerBase
{
    private readonly AppDbContext _db;

    public MatchController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("create")]
    public IActionResult Create([FromBody] CreateMatchRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username)) return BadRequest("User Email is Missing");
        if (string.IsNullOrWhiteSpace(request.MapName)) return BadRequest("Map Name is Missing");
        if (string.IsNullOrWhiteSpace(request.Season)) return BadRequest("Map Name is Missing");
        if (string.IsNullOrWhiteSpace(request.Rank)) return BadRequest("Rank is Missing");
        if (request.RankDivision == 0) return BadRequest("Rank Division Cannot be 0");
        if (string.IsNullOrWhiteSpace(request.MatchResult)) return BadRequest("Match Result is Missing");
        if (string.IsNullOrWhiteSpace(request.Hero_1)) return BadRequest("First Hero is Missing");
        if (string.IsNullOrWhiteSpace(request.TeamBan_1)) return BadRequest("First Hero Banned from Team is Missing");
        if (string.IsNullOrWhiteSpace(request.TeamBan_2)) return BadRequest("Second Hero Banned from Team is Missing");
        if (string.IsNullOrWhiteSpace(request.EnemyTeamBan_1))
            return BadRequest("First Hero Banned from Enemy Team is Missing");
        if (string.IsNullOrWhiteSpace(request.EnemyTeamBan_2))
            return BadRequest("Second Hero Banned from Enemy Team is Missing");


        var newMatch = new Match
        {
            Username = request.Username,
            UploadTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            MapName = request.MapName,
            Season = request.Season,
            Rank = request.Rank,
            RankDivision = request.RankDivision,
            RankPercentage = request.RankPercentage,
            Hero_1 = request.Hero_1,
            Hero_2 = request.Hero_2,
            Hero_3 = request.Hero_3,
            MatchResult = request.MatchResult,
            TeamBan_1 = request.TeamBan_1,
            TeamBan_2 = request.TeamBan_2,
            EnemyTeamBan_1 = request.EnemyTeamBan_1,
            EnemyTeamBan_2 = request.EnemyTeamBan_2,
            TeamNotes = request.TeamNotes,
            EnemyTeamNotes = request.EnemyTeamNotes
        };

        _db.Match.Add(newMatch);
        _db.SaveChanges();
        var response = new GenericResponse()
        {
            ok = true,
            ResponseMessage = "Match created successfully"
        };
        return Ok(JsonSerializer.Serialize(response));
    }

    [HttpPost("get-by-username")]
    public async Task<IActionResult> GetByUsername([FromBody] UsernameRequest request)
    {
        var records = await _db.Match
            .Where(match => match.Username == request.Username)
            .ToListAsync();


        var response = new MatchListResponse()
        {
            Matches = new List<Match>()
        };
        
        if (records.Count != 0)
        {
            response.Matches = new List<Match>(records);
        }

        return Ok(JsonSerializer.Serialize(response));
    }
}

public class CreateMatchRequest
{
    public string Username { get; set; } = "";
    public string MapName { get; set; } = "";
    public string Season { get; set; } = "";
    public string Rank { get; set; } = "";
    public int RankDivision { get; set; } = 0;
    public int RankPercentage { get; set; } = 0;
    public string Hero_1 { get; set; } = "";
    public string Hero_2 { get; set; } = "";
    public string Hero_3 { get; set; } = "";
    public string MatchResult { get; set; } = "";
    public string TeamBan_1 { get; set; } = "";
    public string TeamBan_2 { get; set; } = "";
    public string EnemyTeamBan_1 { get; set; } = "";
    public string EnemyTeamBan_2 { get; set; } = "";
    public string TeamNotes { get; set; } = "";
    public string EnemyTeamNotes { get; set; } = "";
}

public class UsernameRequest
{
    public string Username { get; set; } = "";
}

public class MatchListResponse
{
    public List<Match> Matches { get; set; } = new List<Match>();
}