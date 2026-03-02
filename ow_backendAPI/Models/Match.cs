using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ow_backendAPI.Models;

[Table("game_records", Schema = "data")]
public class Match
{
    public int Id { get; set; } = -1;
    public int UserId { get; set; } = -1;
    public AppUser User { get; set; } = new AppUser();
    public string SubmitTime { get; set; } = "";
    public int MapId { get; set; } = -1;
    public Map Map { get; set; } = new Map();
    public string Season { get; set; } = "";
    public string Rank { get; set; } = "";
    public int RankDivision { get; set; } = -1;
    public int RankPercentage { get; set; } = -1;
    public int Hero1Id { get; set; } = -1;
    public Hero Hero1 { get; set; } = new Hero();
    public int? Hero2Id { get; set; }
    public Hero? Hero2 { get; set; }
    public int? Hero3Id { get; set; }
    public Hero? Hero3 { get; set; }
    public string MatchResult { get; set; } = "";
    public int TeamBan1Id { get; set; } = -1;
    public Hero TeamBan1 { get; set; } = new Hero();
    public int TeamBan2Id { get; set; } = -1;
    public Hero TeamBan2 { get; set; } = new Hero();
    public int EnemyTeamBan1Id { get; set; } = -1;
    public Hero EnemyTeamBan1 { get; set; } = new Hero();
    public int EnemyTeamBan2Id { get; set; } = -1;
    public Hero EnemyTeamBan2 { get; set; } = new Hero();
    public string? TeamNotes { get; set; } = "";
    public string? EnemyTeamNotes { get; set; } = "";
}

public class MatchEntityConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("game_records");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");

        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();


        builder.Property(x => x.SubmitTime).HasColumnName("submit_time");

        builder.Property(x => x.MapId).HasColumnName("map_id");
        builder.HasOne(x => x.Map)
            .WithMany()
            .HasForeignKey(x => x.MapId)
            .IsRequired();

        builder.Property(x => x.Season).HasColumnName("season");

        builder.Property(x => x.Rank).HasColumnName("rank");

        builder.Property(x => x.RankDivision).HasColumnName("rank_division").IsRequired();
        builder.Property(x => x.RankPercentage).HasColumnName("rank_percentage").IsRequired();
        builder.Property(x => x.MatchResult).HasColumnName("match_result").IsRequired();

        builder.Property(x => x.Hero1Id).HasColumnName("hero_1_id");
        builder.HasOne(x => x.Hero1)
            .WithMany()
            .HasForeignKey(x => x.Hero1Id)
            .IsRequired();

        builder.Property(x => x.Hero2Id).HasColumnName("hero_2_id");
        builder.HasOne(x => x.Hero2)
            .WithMany()
            .HasForeignKey(x => x.Hero2Id);

        builder.Property(x => x.Hero3Id)
            .HasColumnName("hero_3_id");


        builder.HasOne(x => x.Hero3)
            .WithMany()
            .HasForeignKey(x => x.Hero3Id);


        builder.Property(x => x.TeamBan1Id).HasColumnName("team_ban_1_id");
        builder.HasOne(x => x.TeamBan1)
            .WithMany()
            .HasForeignKey(x => x.TeamBan1Id)
            .IsRequired();

        builder.Property(x => x.TeamBan2Id).HasColumnName("team_ban_2_id");
        builder.HasOne(x => x.TeamBan2)
            .WithMany()
            .HasForeignKey(x => x.TeamBan2Id)
            .IsRequired();


        builder.Property(x => x.EnemyTeamBan1Id).HasColumnName("enemy_team_ban_1_id");
        builder.HasOne(x => x.EnemyTeamBan1)
            .WithMany()
            .HasForeignKey(x => x.EnemyTeamBan1Id)
            .IsRequired();

        builder.Property(x => x.EnemyTeamBan2Id).HasColumnName("enemy_team_ban_2_id");
        builder.HasOne(x => x.EnemyTeamBan2)
            .WithMany()
            .HasForeignKey(x => x.EnemyTeamBan2Id)
            .IsRequired();

        builder.Property(x => x.TeamNotes).HasColumnName("team_notes");
        builder.Property(x => x.EnemyTeamNotes).HasColumnName("enemy_team_notes");
    }
}