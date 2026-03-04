using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ow_backendAPI.Data;
using ow_backendAPI.Models;

namespace ow_backendAPI.Controllers;

[ApiController]
[Route("owstatistics/api/match")]
[Authorize]
public class MatchController(AppDbContext db) : ControllerBase
{
    // ─── Helpers ───────────────────────────────────────────
    private int? CallerUserId =>
        int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

    private bool CallerIsAdmin =>
        User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

    private bool CallerCanAccess(int targetUserId) =>
        CallerIsAdmin || CallerUserId == targetUserId;
    
    // ─── Create ────────────────────────────────────────────
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateMatchRequest request)
    {
        // Validate caller can create for this userId
        if (!CallerCanAccess(request.UserId))
            return StatusCode(403, new GenericResponse
                { ResponseMessage = "You can only create matches for your own account" });

        if (request.UserId == -1)    return BadRequest(new GenericResponse { ResponseMessage = "User Id is missing" });
        if (request.MapId == -1)     return BadRequest(new GenericResponse { ResponseMessage = "Map Id is missing" });
        if (string.IsNullOrWhiteSpace(request.Season))      return BadRequest(new GenericResponse { ResponseMessage = "Season is missing" });
        if (string.IsNullOrWhiteSpace(request.Rank))        return BadRequest(new GenericResponse { ResponseMessage = "Rank is missing" });
        if (request.RankDivision == 0)                      return BadRequest(new GenericResponse { ResponseMessage = "Rank division cannot be 0" });
        if (string.IsNullOrWhiteSpace(request.MatchResult)) return BadRequest(new GenericResponse { ResponseMessage = "Match result is missing" });
        if (request.Hero1Id == -1)        return BadRequest(new GenericResponse { ResponseMessage = "First hero Id is missing" });
        if (request.TeamBan1Id == -1)     return BadRequest(new GenericResponse { ResponseMessage = "Team ban 1 Id is missing" });
        if (request.TeamBan2Id == -1)     return BadRequest(new GenericResponse { ResponseMessage = "Team ban 2 Id is missing" });
        if (request.EnemyTeamBan1Id == -1) return BadRequest(new GenericResponse { ResponseMessage = "Enemy ban 1 Id is missing" });
        if (request.EnemyTeamBan2Id == -1) return BadRequest(new GenericResponse { ResponseMessage = "Enemy ban 2 Id is missing" });

        var user = await db.AppUsers.FindAsync(request.UserId);
        if (user == null)
            return NotFound(new GenericResponse { ResponseMessage = "User not found" });

        var newMatch = new Match
        {
            Id              = 0,
            UserId          = user.Id,
            MapId           = request.MapId,
            Season          = request.Season,
            Rank            = request.Rank,
            RankDivision    = request.RankDivision,
            RankPercentage  = request.RankPercentage,
            Hero1Id         = request.Hero1Id,
            MatchResult     = request.MatchResult,
            TeamBan1Id      = request.TeamBan1Id,
            TeamBan2Id      = request.TeamBan2Id,
            EnemyTeamBan1Id = request.EnemyTeamBan1Id,
            EnemyTeamBan2Id = request.EnemyTeamBan2Id,
            TeamNotes       = request.TeamNotes,
            EnemyTeamNotes  = request.EnemyTeamNotes
        };

        if (request.Hero2Id is not null and not -1)
        {
            var hero2 = await db.Heroes.FindAsync(request.Hero2Id);
            if (hero2 != null) newMatch.Hero2Id = hero2.Id;
        }

        if (request.Hero3Id is not null and not -1)
        {
            var hero3 = await db.Heroes.FindAsync(request.Hero3Id);
            if (hero3 != null) newMatch.Hero3Id = hero3.Id;
        }

        db.Matches.Add(newMatch);
        await db.SaveChangesAsync();

        return Ok(new GenericResponse { Ok = true, ResponseMessage = "Match created successfully" });
    
    }

    // ─── Get by user id ────────────────────────────────────
    [HttpPost("get-by-user-id")]
    public async Task<IActionResult> GetByUserId([FromBody] UserRequest request)
    {
        if (request.UserId == -1)
            return BadRequest(new GenericResponse { ResponseMessage = "User Id is missing" });

        // Users can only fetch their own matches — admins can fetch anyone's
        if (!CallerCanAccess(request.UserId))
            return StatusCode(403, new GenericResponse
                { ResponseMessage = "You can only retrieve your own matches" });

        var records = await db.Matches
            .Where(m => m.UserId == request.UserId)
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
                Id             = m.Id,
                UserId         = m.UserId,
                SubmitTime     = m.SubmitTime,
                Map            = m.Map,
                Season         = m.Season,
                Rank           = m.Rank,
                RankDivision   = m.RankDivision,
                RankPercentage = m.RankPercentage,
                Hero1          = m.Hero1,
                Hero2          = m.Hero2,
                Hero3          = m.Hero3,
                MatchResult    = m.MatchResult,
                TeamBan1       = m.TeamBan1,
                TeamBan2       = m.TeamBan2,
                EnemyTeamBan1  = m.EnemyTeamBan1,
                EnemyTeamBan2  = m.EnemyTeamBan2,
                TeamNotes      = m.TeamNotes,
                EnemyTeamNotes = m.EnemyTeamNotes
            })
            .ToListAsync();

        return Ok(records);
    }
    
    // ─── Update ────────────────────────────────────────────
    [HttpPut("update/{matchId:int}")]
    public async Task<IActionResult> Update(int matchId, [FromBody] UpdateMatchRequest request)
    {
        var match = await db.Matches.FindAsync(matchId);
        if (match == null)
            return NotFound(new GenericResponse { ResponseMessage = "Match not found" });

        // Ownership check — only the owner or an Admin can modify this match
        if (!CallerCanAccess(match.UserId))
            return StatusCode(403, new GenericResponse
                { ResponseMessage = "You can only modify your own matches" });
        var original = new
        {
            match.MapId,
            match.Season,
            match.Rank,
            match.RankDivision,
            match.RankPercentage,
            match.MatchResult,
            match.Hero1Id,
            match.Hero2Id,
            match.Hero3Id,
            match.TeamBan1Id,
            match.TeamBan2Id,
            match.EnemyTeamBan1Id,
            match.EnemyTeamBan2Id,
            match.TeamNotes,
            match.EnemyTeamNotes
        };
        
        // Only update fields that are explicitly provided
        if (request.MapId is not null and not -1)
            match.MapId = request.MapId.Value;

        if (!string.IsNullOrWhiteSpace(request.Season))
            match.Season = request.Season;

        if (!string.IsNullOrWhiteSpace(request.Rank))
            match.Rank = request.Rank;

        if (request.RankDivision is not null and not 0)
            match.RankDivision = request.RankDivision.Value;

        if (request.RankPercentage is not null)
            match.RankPercentage = request.RankPercentage.Value;

        if (!string.IsNullOrWhiteSpace(request.MatchResult))
            match.MatchResult = request.MatchResult;

        if (request.Hero1Id is not null and not -1)
            match.Hero1Id = request.Hero1Id.Value;

        // Hero2 — supports clearing (pass -1 to remove)
        if (request.Hero2Id is not null)
            match.Hero2Id = request.Hero2Id == -1 ? null : request.Hero2Id;

        // Hero3 — supports clearing (pass -1 to remove)
        if (request.Hero3Id is not null)
            match.Hero3Id = request.Hero3Id == -1 ? null : request.Hero3Id;

        if (request.TeamBan1Id is not null and not -1)
            match.TeamBan1Id = request.TeamBan1Id.Value;

        if (request.TeamBan2Id is not null and not -1)
            match.TeamBan2Id = request.TeamBan2Id.Value;

        if (request.EnemyTeamBan1Id is not null and not -1)
            match.EnemyTeamBan1Id = request.EnemyTeamBan1Id.Value;

        if (request.EnemyTeamBan2Id is not null and not -1)
            match.EnemyTeamBan2Id = request.EnemyTeamBan2Id.Value;

        if (request.TeamNotes is not null)
            match.TeamNotes = request.TeamNotes;

        if (request.EnemyTeamNotes is not null)
            match.EnemyTeamNotes = request.EnemyTeamNotes;

        var hasChanges =
            match.MapId           != original.MapId           ||
            match.Season          != original.Season          ||
            match.Rank            != original.Rank            ||
            match.RankDivision    != original.RankDivision    ||
            match.RankPercentage  != original.RankPercentage  ||
            match.MatchResult     != original.MatchResult     ||
            match.Hero1Id         != original.Hero1Id         ||
            match.Hero2Id         != original.Hero2Id         ||
            match.Hero3Id         != original.Hero3Id         ||
            match.TeamBan1Id      != original.TeamBan1Id      ||
            match.TeamBan2Id      != original.TeamBan2Id      ||
            match.EnemyTeamBan1Id != original.EnemyTeamBan1Id ||
            match.EnemyTeamBan2Id != original.EnemyTeamBan2Id ||
            match.TeamNotes       != original.TeamNotes       ||
            match.EnemyTeamNotes  != original.EnemyTeamNotes;
        
        if (!hasChanges)
            return Ok(new GenericResponse { Ok = true, ResponseMessage = "No changes detected" });

        await db.SaveChangesAsync();

        return Ok(new GenericResponse { Ok = true, ResponseMessage = "Match updated successfully" });
    }
    
    // ─── Delete ────────────────────────────────────────────
    [HttpDelete("delete/{matchId:int}")]
    public async Task<IActionResult> Delete(int matchId)
    {
        var match = await db.Matches.FindAsync(matchId);
        if (match == null)
            return NotFound(new GenericResponse { ResponseMessage = "Match not found" });

        if (!CallerCanAccess(match.UserId))
            return StatusCode(403, new GenericResponse
                { ResponseMessage = "You can only delete your own matches" });

        db.Matches.Remove(match);
        await db.SaveChangesAsync();

        return Ok(new GenericResponse { Ok = true, ResponseMessage = "Match deleted successfully" });
    }
}

public class CreateMatchRequest
{
    public int     UserId          { get; set; } = -1;
    public int     MapId           { get; set; } = -1;
    public string  Season          { get; set; } = "";
    public string  Rank            { get; set; } = "";
    public int     RankDivision    { get; set; } = 0;
    public int     RankPercentage  { get; set; } = 0;
    public int     Hero1Id         { get; set; } = -1;
    public int?    Hero2Id         { get; set; }
    public int?    Hero3Id         { get; set; }
    public string  MatchResult     { get; set; } = "";
    public int     TeamBan1Id      { get; set; } = -1;
    public int     TeamBan2Id      { get; set; } = -1;
    public int     EnemyTeamBan1Id { get; set; } = -1;
    public int     EnemyTeamBan2Id { get; set; } = -1;
    public string? TeamNotes       { get; set; }
    public string? EnemyTeamNotes  { get; set; }
}

public class UpdateMatchRequest
{
    // All fields optional — only provided fields will be updated
    public int?    MapId           { get; set; }
    public string? Season          { get; set; }
    public string? Rank            { get; set; }
    public int?    RankDivision    { get; set; }
    public int?    RankPercentage  { get; set; }
    public int?    Hero1Id         { get; set; }
    public int?    Hero2Id         { get; set; }  // send -1 to clear
    public int?    Hero3Id         { get; set; }  // send -1 to clear
    public string? MatchResult     { get; set; }
    public int?    TeamBan1Id      { get; set; }
    public int?    TeamBan2Id      { get; set; }
    public int?    EnemyTeamBan1Id { get; set; }
    public int?    EnemyTeamBan2Id { get; set; }
    public string? TeamNotes       { get; set; }
    public string? EnemyTeamNotes  { get; set; }
}


public class MatchDto
{
    public int     Id             { get; set; } = -1;
    public int     UserId         { get; set; } = -1;
    public DateTime  SubmitTime     { get; set; } = DateTime.UtcNow;
    public Map     Map            { get; set; } = new();
    public string  Season         { get; set; } = "";
    public string  Rank           { get; set; } = "";
    public int     RankDivision   { get; set; } = -1;
    public int     RankPercentage { get; set; } = -1;
    public Hero    Hero1          { get; set; } = new();
    public Hero?   Hero2          { get; set; }
    public Hero?   Hero3          { get; set; }
    public string  MatchResult    { get; set; } = "";
    public Hero    TeamBan1       { get; set; } = new();
    public Hero    TeamBan2       { get; set; } = new();
    public Hero    EnemyTeamBan1  { get; set; } = new();
    public Hero    EnemyTeamBan2  { get; set; } = new();
    public string? TeamNotes      { get; set; }
    public string? EnemyTeamNotes { get; set; }
}

public class UserRequest
{
    public int UserId { get; set; } = -1;
}