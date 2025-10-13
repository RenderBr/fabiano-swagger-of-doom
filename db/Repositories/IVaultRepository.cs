using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IVaultRepository
{
    Task<Vault> GetByIdAsync(int id);
    Task<List<Vault>> GetByAccountIdAsync(long accountId);
    Task<Vault> GetByChestIdAsync(long accountId, int chestId);
    Task AddAsync(Vault vault);
    Task UpdateAsync(Vault vault);
    Task DeleteAsync(Vault vault);
    Task DeleteByAccountIdAsync(long accountId);
    Task SaveChangesAsync();
}
