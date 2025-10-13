using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class GuildRepository : IGuildRepository
{
    private readonly ServerDbContext _context;

    public GuildRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<GuildEntity> GetByIdAsync(int id)
        => await _context.Guilds.FindAsync(id);

    public async Task<GuildEntity> GetByNameAsync(string name)
        => await _context.Guilds.FirstOrDefaultAsync(g => g.Name == name);

    public async Task<List<GuildEntity>> GetAllAsync()
        => await _context.Guilds.ToListAsync();

    public async Task AddAsync(GuildEntity guild)
    {
        await _context.Guilds.AddAsync(guild);
    }

    public async Task UpdateAsync(GuildEntity guild)
    {
        _context.Guilds.Update(guild);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(GuildEntity guild)
    {
        _context.Guilds.Remove(guild);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
