using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class DailyQuestRepository : IDailyQuestRepository
{
    private readonly ServerDbContext _context;

    public DailyQuestRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<DailyQuest> GetByAccountIdAsync(long accountId)
        => await _context.DailyQuests.FindAsync(accountId);

    public async Task AddAsync(DailyQuest dailyQuest)
    {
        await _context.DailyQuests.AddAsync(dailyQuest);
    }

    public async Task UpdateAsync(DailyQuest dailyQuest)
    {
        _context.DailyQuests.Update(dailyQuest);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(DailyQuest dailyQuest)
    {
        _context.DailyQuests.Remove(dailyQuest);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
