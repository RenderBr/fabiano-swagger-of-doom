using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IClassStatRepository
{
    Task<ClassStat> GetByIdAsync(long accountId, int objectType);
    Task<List<ClassStat>> GetByAccountIdAsync(long accountId);
    Task AddAsync(ClassStat classStat);
    Task UpdateAsync(ClassStat classStat);
    Task DeleteAsync(ClassStat classStat);
    Task DeleteByAccountIdAsync(long accountId);
    Task UpsertAsync(ClassStat classStat);
    Task SaveChangesAsync();
}
