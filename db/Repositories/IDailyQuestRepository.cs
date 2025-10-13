using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IDailyQuestRepository
{
    Task<DailyQuest> GetByAccountIdAsync(long accountId);
    Task AddAsync(DailyQuest dailyQuest);
    Task UpdateAsync(DailyQuest dailyQuest);
    Task DeleteAsync(DailyQuest dailyQuest);
    Task SaveChangesAsync();
}
