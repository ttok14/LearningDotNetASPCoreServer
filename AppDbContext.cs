using LearningServer01.Data;
using Microsoft.EntityFrameworkCore;

namespace LearningServer01
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<PlayerInfo> Players { get; set; }
        public DbSet<StructureInfo> Structures { get; set; }
    }
}
