using LearningServer01.Data;
using Microsoft.EntityFrameworkCore;

namespace LearningServer01.Repositories
{
    public class DbPlayerRepository : IPlayerRepository
    {
        private readonly AppDbContext _context;

        public DbPlayerRepository(AppDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<PlayerInfo> GetPlayerAsync(string nickName)
        {
            return await _context.Players
                .FirstOrDefaultAsync(p => p.ID == nickName);
        }

        public async Task<bool> AddPlayerAsync(PlayerInfo info)
        {
            bool exists = await _context.Players.AnyAsync(p => p.ID == info.ID);

            if (exists)
                return false;

            _context.Players.Add(info);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
