using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface ICharacterRepository : IRepository<Character>
{
    Task<Character> GetByCharacterIdAsync(long accountId, int characterId);
    Task<List<Character>> GetByAccountIdAsync(long accountId);
    Task<List<Character>> GetLiveCharactersByAccountIdAsync(long accountId);
    Task UpdateAsync(Character character);
    Task DeleteAsync(Character character);
}
