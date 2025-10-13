using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class NewsRepository : INewsRepository
{
    private readonly ServerDbContext _context;

    public NewsRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<News> GetByIdAsync(int id)
        => await _context.News.FindAsync(id);

    public async Task<List<News>> GetLatestAsync(int count = 10)
        => await _context.News
            .OrderByDescending(n => n.Date)
            .Take(count)
            .ToListAsync();

    public async Task AddAsync(News news)
    {
        await _context.News.AddAsync(news);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
