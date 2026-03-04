using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ow_backendAPI.Models;

[Table("map_list", Schema = "data")]
public class Map
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Mode { get; set; } = "";
    public int ModeId { get; set; }
}

public class MapEntityConfiguration : IEntityTypeConfiguration<Map>
{
    public void Configure(EntityTypeBuilder<Map> builder)
    {
        builder.ToTable("map_list", schema: "data");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Mode)
            .HasColumnName("mode")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.ModeId)
            .HasColumnName("mode_id")
            .IsRequired();
    }
}