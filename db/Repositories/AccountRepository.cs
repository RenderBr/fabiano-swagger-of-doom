using System.Threading.Tasks;
using db.Models;
using Microsoft.EntityFrameworkCore;

namespace db.Repositories;

public class AccountRepository : Repository<Account>, IAccountRepository
{
    public AccountRepository(ServerDbContext context) : base(context)
    {
    }

    public async Task<Account> GetByIdAsync(int id)
        => await GetByIdAsync((long)id);

    public async Task<Account> GetByUuidAsync(string uuid)
        => await FirstOrDefaultAsync(a => a.Uuid == uuid);

    public async Task<Account> GetByAuthTokenAsync(string authToken)
        => await FirstOrDefaultAsync(a => a.AuthToken == authToken);

    public async Task<Account> VerifyAsync(string uuid, string password)
    {
        var hash = Utils.Sha1(password);
        return await FirstOrDefaultAsync(a => a.Uuid == uuid && a.Password == hash);
    }
}