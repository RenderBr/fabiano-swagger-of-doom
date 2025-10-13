using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account> GetByIdAsync(int id);
    Task<Account> GetByUuidAsync(string uuid);
    Task<Account> GetByAuthTokenAsync(string authToken);
    Task<Account> VerifyAsync(string uuid, string password);
}