using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ow_backendAPI.Models;

[Table("game_records", Schema = "public")]
public class Match
{
    [Key] [Column("id")] public int Id { get; set; }
    [Column("user_email")] public string UserEmail { get; set; } = "";
    [Column("submit_time")] public string UploadTime { get; set; } = "";
    [Column("map_name")] public string MapName { get; set; } = "";
    [Column("season")] public string Season { get; set; } = "";
    [Column("rank")] public string Rank { get; set; } = "";
    [Column("rank_division")] public int RankDivision { get; set; } = 0;
    [Column("rank_percentage")] public int RankPercentage { get; set; } = 0;
    [Column("hero_1")] public string Hero_1 { get; set; } = "";
    [Column("hero_2")] public string Hero_2 { get; set; } = "";
    [Column("hero_3")] public string Hero_3 { get; set; } = "";
    [Column("match_result")] public string MatchResult { get; set; } = "";
    [Column("team_ban_1")] public string TeamBan_1 { get; set; } = "";
    [Column("team_ban_2")] public string TeamBan_2 { get; set; } = "";
    [Column("enemy_team_ban_1")] public string EnemyTeamBan_1 { get; set; } = "";
    [Column("enemy_team_ban_2")] public string EnemyTeamBan_2 { get; set; } = "";
    [Column("team_notes")] public string TeamNotes { get; set; } = "";
    [Column("enemy_team_notes")] public string EnemyTeamNotes { get; set; } = "";
}