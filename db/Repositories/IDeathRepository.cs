using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IDeathRepository
{
    Task<Death> GetByIdAsync(long id);
    Task<List<Death>> GetByAccountIdAsync(long accountId);
    Task<Death> GetByAccountIdAndCharacterIdAsync(long accountId, int characterId);
    Task AddAsync(Death death);
    Task SaveChangesAsync();
}
