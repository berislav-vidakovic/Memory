using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("users")]
    public class User
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("password_hash")]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [Column("login")]
        [MaxLength(100)]
        public string Login { get; set; } = string.Empty;

        [Column("full_name")]
        [MaxLength(255)]
        public required string FullName { get; set; }

        [Column("isonline")]
        public bool IsOnline { get; set; } = false;
    }
}
