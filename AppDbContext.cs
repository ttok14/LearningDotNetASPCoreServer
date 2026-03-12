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
        public DbSet<BattleLogInfo> BattleLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BattleLogInfo>()
                  .HasOne(b => b.DefenderPlayer)      
                  .WithMany()        
                  .HasForeignKey(b => b.DefenderId)   
                  .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<BattleLogInfo>()
                .HasOne(b => b.AttackerPlayer)
                .WithMany()                       
                .HasForeignKey(b => b.AttackerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
