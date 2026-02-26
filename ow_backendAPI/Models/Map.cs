using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ow_backendAPI.Models;

[Table("map_list", Schema = "data")]
public class Map
{
    [Key] [Column("id")] public int Id { get; set; }

    [Column("name")] public string Name { get; set; } = "";

    [Column("mode")] public string Mode { get; set; } = "";

    [Column("mode_id")] public int ModeId { get; set; } 
}