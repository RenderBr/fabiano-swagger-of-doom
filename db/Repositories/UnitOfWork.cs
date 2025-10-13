using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using db.Models;
using db.Repositories;

namespace db.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ServerDbContext _context;
    private IDbContextTransaction _transaction;

    private IAccountRepository _accounts;
    private ICharacterRepository _characters;
    private IGuildRepository _guilds;
    private IVaultRepository _vaults;
    private IStatRepository _stats;
    private IPetRepository _pets;
    private IDailyQuestRepository _dailyQuests;
    private IClassStatRepository _classStats;
    private IDeathRepository _deaths;
    private INewsRepository _news;
    private IBackpackRepository _backpacks;
    private IGiftCodeRepository _giftCodes;

    public UnitOfWork(ServerDbContext context)
    {
        _context = context;
    }

    public IAccountRepository Accounts => _accounts ??= new AccountRepository(_context);
    public ICharacterRepository Characters => _characters ??= new CharacterRepository(_context);
    public IGuildRepository Guilds => _guilds ??= new GuildRepository(_context);
    public IVaultRepository Vaults => _vaults ??= new VaultRepository(_context);
    public IStatRepository Stats => _stats ??= new StatRepository(_context);
    public IPetRepository Pets => _pets ??= new PetRepository(_context);
    public IDailyQuestRepository DailyQuests => _dailyQuests ??= new DailyQuestRepository(_context);
    public IClassStatRepository ClassStats => _classStats ??= new ClassStatRepository(_context);
    public IDeathRepository Deaths => _deaths ??= new DeathRepository(_context);
    public INewsRepository News => _news ??= new NewsRepository(_context);
    public IBackpackRepository Backpacks => _backpacks ??= new BackpackRepository(_context);
    public IGiftCodeRepository GiftCodes => _giftCodes ??= new GiftCodeRepository(_context);
    public ServerDbContext Context => _context;

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _context.Dispose();
        if (_transaction != null)
        {
            _transaction.Dispose();
        }
    }
}