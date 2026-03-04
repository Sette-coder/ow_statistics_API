using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ow_backendAPI.Models;

[Table("app_users", Schema = "data")]
public class AppUser
{
    [Key] [Column("id")] [DatabaseGenerated(DatabaseGeneratedOption.Identity)]public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column("username")]public string Username { get; set; } = "";

    [Required]
    [MaxLength(100)]
    [Column("email")] public string Email { get; set; } = "";

    [Required]
    [Column("role")] public string Role { get; set; } = "";

    [Required]
    [Column("password_hash")] public string PasswordHash { get; set; } = "";
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AppUserEntityConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("app_users", schema:"data");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.Username).HasColumnName("username").IsRequired();
        builder.Property(x => x.Email).HasColumnName("email").IsRequired();
        builder.Property(x => x.PasswordHash).HasColumnName("password_hash").IsRequired();
        builder.Property(x => x.CreatedAt).HasColumnName("created_at");
        builder.HasIndex(x => x.Username).IsUnique();
        builder.HasIndex(x => x.Email).IsUnique();
    }
}