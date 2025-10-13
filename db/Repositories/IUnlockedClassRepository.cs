using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IUnlockedClassRepository : IRepository<UnlockedClass>
{
    public Task<List<UnlockedClass>> GetUserClassesAsync(long accId);
}