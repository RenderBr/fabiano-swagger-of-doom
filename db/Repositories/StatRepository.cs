using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class StatRepository : IStatRepository
{
    private readonly ServerDbContext _context;

    public StatRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<Stat> GetByAccountIdAsync(long accountId)
        => await _context.Stats.FindAsync(accountId);

    public async Task AddAsync(Stat stat)
    {
        await _context.Stats.AddAsync(stat);
    }

    public async Task UpdateAsync(Stat stat)
    {
        _context.Stats.Update(stat);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Stat stat)
    {
        _context.Stats.Remove(stat);
        await Task.CompletedTask;
    }

    public async Task DeleteByAccountIdAsync(long accountId)
    {
        var stat = await GetByAccountIdAsync(accountId);
        if (stat != null)
        {
            _context.Stats.Remove(stat);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
