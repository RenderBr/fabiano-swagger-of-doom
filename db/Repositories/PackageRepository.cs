using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class PackageRepository : Repository<Package>, IPackageRepository
{
    public PackageRepository(ServerDbContext context) : base(context) { }

    public async Task<Package> GetByIdAsync(int id)
    {
        return await _context.Packages.FindAsync(id);
    }

    public async Task<List<Package>> GetActivePackagesAsync()
    {
        return await _context.Packages.Where(p => p.EndDate >= DateTime.UtcNow).ToListAsync();
    }
}