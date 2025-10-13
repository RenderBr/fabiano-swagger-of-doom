using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IGlobalNewsRepository
{
    Task<GlobalNews> GetByIdAsync(int id);
    Task<List<GlobalNews>> GetLatestAsync(int count = 10);
    Task AddAsync(GlobalNews news);
    Task SaveChangesAsync();
}