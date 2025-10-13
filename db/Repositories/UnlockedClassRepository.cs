using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class UnlockedClassRepository : IUnlockedClassRepository
{
    private readonly ServerDbContext _context;

    public UnlockedClassRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<UnlockedClass> GetByIdAsync(object id)
    {
        return await _context.UnlockedClasses.FindAsync(id);
    }

    public async Task<IEnumerable<UnlockedClass>> GetAllAsync()
    {
        return await _context.UnlockedClasses.ToListAsync();
    }

    public async Task<IEnumerable<UnlockedClass>> FindAsync(Expression<Func<UnlockedClass, bool>> predicate)
    {
        return await _context.UnlockedClasses.Where(predicate).ToListAsync();
    }

    public async Task<UnlockedClass> FirstOrDefaultAsync(Expression<Func<UnlockedClass, bool>> predicate)
    {
        return await _context.UnlockedClasses.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(UnlockedClass entity)
    {
        await _context.UnlockedClasses.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<UnlockedClass> entities)
    {
        await _context.UnlockedClasses.AddRangeAsync(entities);
    }

    public void Update(UnlockedClass entity)
    {
        _context.UnlockedClasses.Update(entity);
    }

    public void UpdateRange(IEnumerable<UnlockedClass> entities)
    {
        _context.UnlockedClasses.UpdateRange(entities);
    }

    public void Remove(UnlockedClass entity)
    {
        _context.UnlockedClasses.Remove(entity);
    }

    public void RemoveRange(IEnumerable<UnlockedClass> entities)
    {
        _context.UnlockedClasses.RemoveRange(entities);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<List<UnlockedClass>> GetUserClassesAsync(long accId)
    {
        return await _context.UnlockedClasses
            .Where(uc => uc.AccId == accId)
            .ToListAsync();
    }
}