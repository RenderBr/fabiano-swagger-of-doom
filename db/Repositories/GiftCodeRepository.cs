using System.Threading.Tasks;
using db.Models;

namespace db.Repositories;

public class GiftCodeRepository : Repository<GiftCode>, IGiftCodeRepository
{
    public GiftCodeRepository(ServerDbContext context) : base(context)
    {
    }

    public async Task<GiftCode> GetByCodeAsync(string code)
        => await FirstOrDefaultAsync(gc => gc.Code == code);
}