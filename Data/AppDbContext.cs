using Microsoft.EntityFrameworkCore;

namespace LearningServer01.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        
        // Players 테이블 데이터 
        public DbSet<PlayerInfo> Players { get; set; }
    }
}
