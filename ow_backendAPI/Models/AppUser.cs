using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ow_backendAPI.Models;

[Table("app_users", Schema = "data")]
public class AppUser
{
    [Key] [Column("id")] public int Id { get; set; }

    [Column("username")] public string Username { get; set; } = "";

    [Column("email")] public string Email { get; set; } = "";

    [Column("role")] public string Role { get; set; } = "";

    [Column("password")] public string Password { get; set; } = "";
}