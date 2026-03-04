using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ow_backendAPI.Models;

[Table("game_records", Schema = "data")]
public class Match
{
    public int Id { get; set; }
    public int UserId { get; set; } = -1;
    public int MapId { get; set; } = -1;
    public DateTime SubmitTime { get; set; } = DateTime.UtcNow;
    public string Season { get; set; } = "";
    public string Rank { get; set; } = "";
    public int RankDivision { get; set; } = -1;
    public int RankPercentage { get; set; } = -1;
    public int Hero1Id { get; set; } = -1;
    public int TeamBan1Id { get; set; } = -1;
    public int TeamBan2Id { get; set; } = -1;
    public int EnemyTeamBan1Id { get; set; } = -1;
    public int EnemyTeamBan2Id { get; set; } = -1;
    public string MatchResult { get; set; } = "";

    // Optional fields
    public int? Hero2Id { get; set; }
    public int? Hero3Id { get; set; }
    public string? TeamNotes { get; set; }
    public string? EnemyTeamNotes { get; set; }

    // Navigation properties — null! tells compiler EF Core will always load these
    public AppUser User { get; set; } = null!;
    public Map Map { get; set; } = null!;
    public Hero Hero1 { get; set; } = null!;
    public Hero TeamBan1 { get; set; } = null!;
    public Hero TeamBan2 { get; set; } = null!;
    public Hero EnemyTeamBan1 { get; set; } = null!;
    public Hero EnemyTeamBan2 { get; set; } = null!;

    // Optional navigation properties — null because they may not exist
    public Hero? Hero2 { get; set; }
    public Hero? Hero3 { get; set; }
}

public class MatchEntityConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.ToTable("game_records");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(x => x.UserId).HasColumnName("user_id");
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .IsRequired();


        builder.Property(x => x.SubmitTime)
            .HasColumnName("submit_time")
            .HasColumnType("timestamp with time zone");
        

        builder.Property(x => x.MapId).HasColumnName("map_id");
        builder.HasOne(x => x.Map)
            .WithMany()
            .HasForeignKey(x => x.MapId)
            .IsRequired();

        builder.Property(x => x.Season).HasColumnName("season").HasMaxLength(20);

        builder.Property(x => x.Rank).HasColumnName("rank").HasMaxLength(20);

        builder.Property(x => x.RankDivision).HasColumnName("rank_division").IsRequired();
        builder.Property(x => x.RankPercentage).HasColumnName("rank_percentage").IsRequired();
        builder.Property(x => x.MatchResult).HasColumnName("match_result").HasMaxLength(10).IsRequired();

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

        builder.Property(x => x.TeamNotes).HasColumnName("team_notes").HasMaxLength(100);
        builder.Property(x => x.EnemyTeamNotes).HasColumnName("enemy_team_notes").HasMaxLength(100);
    }
}