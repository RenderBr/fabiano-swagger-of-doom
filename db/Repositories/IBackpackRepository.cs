using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IBackpackRepository
{
    Task<Backpack> GetByIdAsync(long accountId, int characterId);
    Task<List<Backpack>> GetByAccountIdAsync(long accountId);
    Task AddAsync(Backpack backpack);
    Task UpdateAsync(Backpack backpack);
    Task DeleteAsync(Backpack backpack);
    Task SaveChangesAsync();
}
