using AuthServer.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users{ get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens{ get; set; } = null!;
    }

}

