using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class ClassStatRepository : IClassStatRepository
{
    private readonly ServerDbContext _context;

    public ClassStatRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<ClassStat> GetByIdAsync(long accountId, int objectType)
        => await _context.ClassStats.FindAsync(accountId, objectType);

    public async Task<List<ClassStat>> GetByAccountIdAsync(long accountId)
        => await _context.ClassStats
            .Where(cs => cs.AccountId == accountId)
            .ToListAsync();

    public async Task AddAsync(ClassStat classStat)
    {
        await _context.ClassStats.AddAsync(classStat);
    }

    public async Task UpdateAsync(ClassStat classStat)
    {
        _context.ClassStats.Update(classStat);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(ClassStat classStat)
    {
        _context.ClassStats.Remove(classStat);
        await Task.CompletedTask;
    }

    public async Task DeleteByAccountIdAsync(long accountId)
    {
        var classStats = await _context.ClassStats
            .Where(cs => cs.AccountId == accountId)
            .ToListAsync();
        _context.ClassStats.RemoveRange(classStats);
    }

    public async Task UpsertAsync(ClassStat classStat)
    {
        var existing = await GetByIdAsync(classStat.AccountId, classStat.ObjectType);
        if (existing != null)
        {
            existing.BestLevel = (byte)Math.Max(existing.BestLevel, classStat.BestLevel);
            existing.BestFame = Math.Max(existing.BestFame, classStat.BestFame);
            _context.ClassStats.Update(existing);
        }
        else
        {
            await _context.ClassStats.AddAsync(classStat);
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
