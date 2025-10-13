using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class VaultRepository : IVaultRepository
{
    private readonly ServerDbContext _context;

    public VaultRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<Vault> GetByIdAsync(int id)
        => await _context.Vaults.FindAsync(id);

    public async Task<List<Vault>> GetByAccountIdAsync(long accountId)
        => await _context.Vaults
            .Where(v => v.AccountId == accountId)
            .ToListAsync();

    public async Task<Vault> GetByChestIdAsync(long accountId, int chestId)
        => await _context.Vaults
            .FirstOrDefaultAsync(v => v.AccountId == accountId && v.ChestId == chestId);

    public async Task AddAsync(Vault vault)
    {
        await _context.Vaults.AddAsync(vault);
    }

    public async Task UpdateAsync(Vault vault)
    {
        _context.Vaults.Update(vault);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Vault vault)
    {
        _context.Vaults.Remove(vault);
        await Task.CompletedTask;
    }

    public async Task DeleteByAccountIdAsync(long accountId)
    {
        var vaults = await _context.Vaults
            .Where(v => v.AccountId == accountId)
            .ToListAsync();
        _context.Vaults.RemoveRange(vaults);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
