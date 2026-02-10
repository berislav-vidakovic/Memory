using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Health> Healths { get; set; } = null!;
        public DbSet<User> Users{ get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens{ get; set; } = null!;
    }

}

