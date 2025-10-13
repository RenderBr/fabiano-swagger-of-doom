using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IStatRepository
{
    Task<Stat> GetByAccountIdAsync(long accountId);
    Task AddAsync(Stat stat);
    Task UpdateAsync(Stat stat);
    Task DeleteAsync(Stat stat);
    Task DeleteByAccountIdAsync(long accountId);
    Task SaveChangesAsync();
}
