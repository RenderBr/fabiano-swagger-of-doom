using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class ClientErrorRepository : IClientErrorRepository
{
    private readonly ServerDbContext _context;

    public ClientErrorRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<ClientError> GetByIdAsync(object id)
    {
        return await _context.ClientErrors.FindAsync(id);
    }

    public async Task<IEnumerable<ClientError>> GetAllAsync()
    {
        return await _context.ClientErrors.ToListAsync();
    }

    public async Task<IEnumerable<ClientError>> FindAsync(Expression<Func<ClientError, bool>> predicate)
    {
        return await _context.ClientErrors.Where(predicate).ToListAsync();
    }

    public Task<ClientError> FirstOrDefaultAsync(Expression<Func<ClientError, bool>> predicate)
    {
        return _context.ClientErrors.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(ClientError entity)
    {
        await _context.ClientErrors.AddAsync(entity);
    }

    public Task AddRangeAsync(IEnumerable<ClientError> entities)
    {
        return _context.ClientErrors.AddRangeAsync(entities);
    }

    public void Update(ClientError entity)
    {
        _context.ClientErrors.Update(entity);
    }

    public void UpdateRange(IEnumerable<ClientError> entities)
    {
        _context.ClientErrors.UpdateRange(entities);
    }

    public void Remove(ClientError entity)
    {
        _context.ClientErrors.Remove(entity);
    }

    public void RemoveRange(IEnumerable<ClientError> entities)
    {
        _context.ClientErrors.RemoveRange(entities);
    }

    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}