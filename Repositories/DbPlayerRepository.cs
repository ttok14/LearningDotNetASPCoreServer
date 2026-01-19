using LearningServer01.Data;
using Microsoft.AspNetCore.Components.Web;
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

        public async Task<PlayerInfo> GetPlayerAsync(string id)
        {
            return await _context.Players.FirstOrDefaultAsync(p => p.ID == id);
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

        public async Task<bool> AddGold(string userId, int amount)
        {
            return await _context.Players
                .Where(p => p.ID == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.Gold, p => p.Gold + amount)) > 0;
        }

        public async Task<bool> AddWood(string userId, int amount)
        {
            return await _context.Players
                .Where(p => p.ID == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.Wood, p => p.Wood + amount)) > 0;
        }

        public async Task<bool> AddFood(string userId, int amount)
        {
            return await _context.Players
                .Where(p => p.ID == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(p => p.Food, p => p.Food + amount)) > 0;
        }
    }
}
