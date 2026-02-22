using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ow_backendAPI.Models;

[Table("hero_list", Schema = "public")]
public class Hero
{
    [Key] [Column("id")] public int Id { get; set; }

    [Column("name")] public string Name { get; set; }

    [Column("role")] public string Role { get; set; }
}