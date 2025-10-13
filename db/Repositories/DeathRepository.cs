using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class DeathRepository : IDeathRepository
{
    private readonly ServerDbContext _context;

    public DeathRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<Death> GetByIdAsync(long id)
        => await _context.Deaths.FindAsync(id);

    public async Task<List<Death>> GetByAccountIdAsync(long accountId)
        => await _context.Deaths
            .Where(d => d.AccountId == accountId)
            .OrderByDescending(d => d.Time)
            .ToListAsync();

    public async Task<Death> GetByAccountIdAndCharacterIdAsync(long accountId, int characterId)
        => await _context.Deaths
            .Where(d => d.AccountId == accountId && d.CharacterId == characterId)
            .OrderByDescending(d => d.Time)
            .FirstOrDefaultAsync();

    public async Task AddAsync(Death death)
    {
        await _context.Deaths.AddAsync(death);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
