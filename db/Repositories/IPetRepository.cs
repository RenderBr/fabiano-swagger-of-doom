using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IPetRepository
{
    Task<Pet> GetByIdAsync(int id);
    Task<Pet> GetByPetIdAsync(long accountId, int petId);
    Task<List<Pet>> GetByAccountIdAsync(long accountId);
    Task AddAsync(Pet pet);
    Task UpdateAsync(Pet pet);
    Task DeleteAsync(Pet pet);
    Task DeleteByAccountIdAsync(long accountId);
    Task SaveChangesAsync();
    Task<List<Pet>> GetAllAsync();
}
