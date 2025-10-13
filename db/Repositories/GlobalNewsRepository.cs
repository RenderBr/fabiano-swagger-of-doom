using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class GlobalNewsRepository : IGlobalNewsRepository
{
    private readonly ServerDbContext _context;

    public GlobalNewsRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<GlobalNews> GetByIdAsync(int id)
    {
        return await _context.GlobalNews.FindAsync(id);
    }

    public async Task<List<GlobalNews>> GetLatestAsync(int count = 10)
        => await _context.GlobalNews
            .OrderByDescending(n => n.Date)
            .Take(count)
            .ToListAsync();

    public async Task AddAsync(GlobalNews news)
    {
        await _context.GlobalNews.AddAsync(news);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}