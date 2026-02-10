using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    [Table("refresh_tokens")]
    public class RefreshToken
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        [Required]
        [Column("token")]
        [MaxLength(255)]
        public string Token { get; set; } = string.Empty;

        [Required]
        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        // Optional navigation property
        [ForeignKey("UserId")]
        public User? User { get; set; }

        // Convenience property (not mapped to DB)
        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}
