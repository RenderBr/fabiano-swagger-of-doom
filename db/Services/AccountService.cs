using System;
using System.Threading.Tasks;
using db.Models;
using db.Repositories;
using MySqlConnector;

namespace db.Services;

public class AccountService
{
    private readonly IUnitOfWork _unitOfWork;

    public AccountService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Account> VerifyAsync(string uuid, string password)
    {
        return await _unitOfWork.Accounts.VerifyAsync(uuid, password);
    }

    public async Task RegisterAsync(Account acc)
    {
        await _unitOfWork.Accounts.AddAsync(acc);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<bool> CheckAccountInUse(Account acc)
    {
        var account = await _unitOfWork.Accounts.GetByIdAsync(long.Parse(acc.AccountId));

        if (account == null)
        {
            return false;
        }

        if (acc.LastSeen == DateTime.MinValue)
        {
            return false;
        }

        var timeInSec = 600 - (int)(DateTime.UtcNow - acc.LastSeen).TotalSeconds;
        return account.AccountInUse && timeInSec > 0;
    }
}