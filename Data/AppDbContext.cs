using Microsoft.EntityFrameworkCore;

namespace LearningServer01.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<PlayerInfo> Players { get; set; }
    }
}
