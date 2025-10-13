using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class BackpackRepository : IBackpackRepository
{
    private readonly ServerDbContext _context;

    public BackpackRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<Backpack> GetByIdAsync(long accountId, int characterId)
        => await _context.Backpacks.FindAsync(accountId, characterId);

    public async Task<List<Backpack>> GetByAccountIdAsync(long accountId)
        => await _context.Backpacks
            .Where(b => b.AccountId == accountId)
            .ToListAsync();

    public async Task AddAsync(Backpack backpack)
    {
        await _context.Backpacks.AddAsync(backpack);
    }

    public async Task UpdateAsync(Backpack backpack)
    {
        _context.Backpacks.Update(backpack);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Backpack backpack)
    {
        _context.Backpacks.Remove(backpack);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
