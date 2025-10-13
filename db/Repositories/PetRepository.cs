using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class PetRepository : IPetRepository
{
    private readonly ServerDbContext _context;

    public PetRepository(ServerDbContext context)
    {
        _context = context;
    }

    public async Task<Pet> GetByIdAsync(int id)
        => await _context.Pets.FindAsync(id);

    public async Task<Pet> GetByPetIdAsync(long accountId, int petId)
        => await _context.Pets
            .FirstOrDefaultAsync(p => p.AccountId == accountId && p.PetId == petId);

    public async Task<List<Pet>> GetByAccountIdAsync(long accountId)
        => await _context.Pets
            .Where(p => p.AccountId == accountId)
            .ToListAsync();

    public async Task AddAsync(Pet pet)
    {
        await _context.Pets.AddAsync(pet);
    }

    public async Task UpdateAsync(Pet pet)
    {
        _context.Pets.Update(pet);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Pet pet)
    {
        _context.Pets.Remove(pet);
        await Task.CompletedTask;
    }

    public async Task DeleteByAccountIdAsync(long accountId)
    {
        var pets = await _context.Pets
            .Where(p => p.AccountId == accountId)
            .ToListAsync();
        _context.Pets.RemoveRange(pets);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<List<Pet>> GetAllAsync()
        => await _context.Pets.ToListAsync();
}