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
        public DbSet<EntityItemInfo> Entities { get; set; }
        public DbSet<UserItem> UserItems { get; set; }
        public DbSet<DeploymentSlot> DeploymentSlots { get; set; }
    }
}
