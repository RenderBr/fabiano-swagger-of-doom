using System;
using System.Threading.Tasks;
using db.Models;
using db.Repositories;

namespace db.Repositories;

public interface IUnitOfWork : IDisposable
{
    IAccountRepository Accounts { get; }
    ICharacterRepository Characters { get; }
    IGuildRepository Guilds { get; }
    IVaultRepository Vaults { get; }
    IStatRepository Stats { get; }
    IPetRepository Pets { get; }
    IDailyQuestRepository DailyQuests { get; }
    IClassStatRepository ClassStats { get; }
    IDeathRepository Deaths { get; }
    INewsRepository News { get; }
    IBackpackRepository Backpacks { get; }
    IGiftCodeRepository GiftCodes { get; }
    ServerDbContext Context { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}