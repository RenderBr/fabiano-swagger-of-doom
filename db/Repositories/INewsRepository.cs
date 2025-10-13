using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface INewsRepository
{
    Task<News> GetByIdAsync(int id);
    Task<List<News>> GetLatestAsync(int count = 10);
    Task AddAsync(News news);
    Task SaveChangesAsync();
}
