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
        public async Task<PlayerInfo> GetPlayer(string nickName)
        {
            return await _context.Players
                .FirstOrDefaultAsync(p => p.NickName == nickName);
        }

        public async Task<bool> AddPlayer(PlayerInfo info)
        {
            bool exists = await _context.Players.AnyAsync(p => p.NickName == info.NickName);

            if (exists)
                return false;

            _context.Players.Add(info);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
