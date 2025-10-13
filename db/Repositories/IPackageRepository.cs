using System.Collections.Generic;
using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IPackageRepository : IRepository<Package>
{
    Task<Package> GetByIdAsync(int id);
    Task<List<Package>> GetActivePackagesAsync();
}