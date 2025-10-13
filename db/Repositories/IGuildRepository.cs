using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IGuildRepository
{
    Task<GuildEntity> GetByIdAsync(int id);
    Task<GuildEntity> GetByNameAsync(string name);
    Task<List<GuildEntity>> GetAllAsync();
    Task AddAsync(GuildEntity guild);
    Task UpdateAsync(GuildEntity guild);
    Task DeleteAsync(GuildEntity guild);
    Task SaveChangesAsync();
}
