using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public interface IGiftCodeRepository : IRepository<GiftCode>
{
    Task<GiftCode> GetByCodeAsync(string code);
}