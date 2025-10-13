using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class CharacterRepository : Repository<Character>, ICharacterRepository
{
    public CharacterRepository(ServerDbContext context) : base(context)
    {
    }

    public async Task<Character> GetByCharacterIdAsync(long accountId, int characterId)
        => await FirstOrDefaultAsync(c => c.AccountId == accountId && c.CharacterId == characterId);

    public async Task<List<Character>> GetByAccountIdAsync(long accountId)
        => (await FindAsync(c => c.AccountId == accountId)).ToList();

    public async Task<List<Character>> GetLiveCharactersByAccountIdAsync(long accountId)
        => (await FindAsync(c => c.AccountId == accountId && !c.Dead)).ToList();

    public async Task UpdateAsync(Character character)
    {
        Update(character);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Character character)
    {
        Remove(character);
        await Task.CompletedTask;
    }
}
