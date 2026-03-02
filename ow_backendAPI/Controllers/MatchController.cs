using System.Numerics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;

[ApiController]
[Route("owstatistics/api/match")]
public class MatchController(AppDbContext db) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateMatchRequest request)
    {
        if (request.UserId == -1) return BadRequest("User Id is Missing");
        if (request.MapId == -1) return BadRequest("Map ID is Missing");
        if (string.IsNullOrWhiteSpace(request.Season)) return BadRequest("Season is Missing");
        if (string.IsNullOrWhiteSpace(request.Rank)) return BadRequest("Rank is Missing");
        if (request.RankDivision == 0) return BadRequest("Rank Division Cannot be 0");
        if (string.IsNullOrWhiteSpace(request.MatchResult)) return BadRequest("Match Result is Missing");
        if (request.Hero1Id == -1) return BadRequest("First Hero ID is Missing");
        if (request.TeamBan1Id == -1) return BadRequest("First Hero Banned ID from Team is Missing");
        if (request.TeamBan2Id == -1) return BadRequest("Second Hero Banned ID from Team is Missing");
        if (request.EnemyTeamBan1Id == -1)
            return BadRequest("First Hero Banned from Enemy Team ID is Missing");
        if (request.EnemyTeamBan2Id == -1)
            return BadRequest("Second Hero Banned from Enemy Team ID is Missing");

        var user = await db.AppUsers.Where(u => u.Id == request.UserId).SingleOrDefaultAsync();

        if (user == null)
        {
            return BadRequest("User not found");
        }

        var newMatch = new Match
        {
            UserId = user.Id,
            SubmitTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            MapId = request.MapId,
            Season = request.Season,
            Rank = request.Rank,
            RankDivision = request.RankDivision,
            RankPercentage = request.RankPercentage,
            Hero1Id = request.Hero1Id,
            MatchResult = request.MatchResult,
            TeamBan1Id = request.TeamBan1Id,
            TeamBan2Id = request.TeamBan2Id,
            EnemyTeamBan1Id = request.EnemyTeamBan1Id,
            EnemyTeamBan2Id = request.EnemyTeamBan2Id,
            TeamNotes = request.TeamNotes,
            EnemyTeamNotes = request.EnemyTeamNotes
        };

        if (request.Hero2Id != null && request.Hero2Id != -1)
        {
            var hero2 = await db.Hero.Where(h => h.Id == request.Hero2Id).SingleOrDefaultAsync();
            if (hero2 != null)
            {
                newMatch.Hero2Id = hero2.Id;
            }
        }

        if (request.Hero3Id != null && request.Hero3Id != -1)
        {
            var hero3 = await db.Hero.Where(h => h.Id == request.Hero3Id).SingleOrDefaultAsync();
            if (hero3 != null)
            {
                newMatch.Hero3Id = hero3.Id;
            }
        }

        db.Match.Add(newMatch);
        await db.SaveChangesAsync();
        var response = new GenericResponse()
        {
            ok = true,
            ResponseMessage = "Match created successfully"
        };
        return Ok(response);
    }

    [HttpPost("get-by-user-id")]
    public async Task<IActionResult> GetByUserId([FromBody] UserRequest request)
    {
        var records = await db.Match
            .Where(match => match.UserId == request.UserId)
            .Include(m => m.Map)
            .Include(m => m.Hero1)
            .Include(m => m.Hero2)
            .Include(m => m.Hero3)
            .Include(m => m.TeamBan1)
            .Include(m => m.TeamBan2)
            .Include(m => m.EnemyTeamBan1)
            .Include(m => m.EnemyTeamBan2)
            .Select(m => new MatchDto
            {
                Id = m.Id,
                UserId = m.UserId,
                SubmitTime = m.SubmitTime,
                Map = m.Map,
                Season = m.Season,
                Rank = m.Rank,
                RankDivision = m.RankDivision,
                RankPercentage = m.RankPercentage,
                Hero1 = m.Hero1,
                Hero2 = m.Hero2,
                Hero3 = m.Hero3,
                MatchResult = m.MatchResult,
                TeamBan1 = m.TeamBan1,
                TeamBan2 = m.TeamBan2,
                EnemyTeamBan1 = m.EnemyTeamBan1,
                EnemyTeamBan2 = m.EnemyTeamBan2,
                TeamNotes = m.TeamNotes,
                EnemyTeamNotes = m.EnemyTeamNotes
            })
            .ToListAsync();


        var response = new List<MatchDto>();
        if (records.Count != 0)
        {
            response = new List<MatchDto>(records);
        }

        return Ok(response);
    }
}

public class CreateMatchRequest
{
    public int UserId { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public string Season { get; set; } = "";
    public string Rank { get; set; } = "";
    public int RankDivision { get; set; } = 0;
    public int RankPercentage { get; set; } = 0;
    public int Hero1Id { get; set; } = -1;
    public int? Hero2Id { get; set; }
    public int? Hero3Id { get; set; }
    public string MatchResult { get; set; } = "";
    public int TeamBan1Id { get; set; } = -1;
    public int TeamBan2Id { get; set; } = -1;
    public int EnemyTeamBan1Id { get; set; } = -1;
    public int EnemyTeamBan2Id { get; set; } = -1;
    public string TeamNotes { get; set; } = "";
    public string EnemyTeamNotes { get; set; } = "";
}

public class MatchDto
{
    public int Id { get; set; } = -1;
    public int UserId { get; set; } = -1;
    public string SubmitTime { get; set; } = "";
    public Map Map { get; set; } = new Map();
    public string Season { get; set; } = "";
    public string Rank { get; set; } = "";
    public int RankDivision { get; set; } = -1;
    public int RankPercentage { get; set; } = -1;
    public Hero Hero1 { get; set; } = new Hero();
    public Hero? Hero2 { get; set; }
    public Hero? Hero3 { get; set; }
    public string MatchResult { get; set; } = "";
    public Hero TeamBan1 { get; set; }= new Hero();
    public Hero TeamBan2 { get; set; }= new Hero();
    public Hero EnemyTeamBan1 { get; set; }= new Hero();
    public Hero EnemyTeamBan2 { get; set; }= new Hero();
    public string? TeamNotes { get; set; }
    public string? EnemyTeamNotes { get; set; }
}

public class UserRequest
{
    public int UserId { get; set; } = -1;
}