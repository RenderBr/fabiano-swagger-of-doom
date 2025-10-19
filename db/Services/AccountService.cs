using System;
using System.Threading.Tasks;
using db.Models;
using db.Repositories;
using Microsoft.Extensions.Logging;

namespace db.Services;

public class AccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountService> _logger;
    
    public AccountService(IUnitOfWork unitOfWork, ILogger<AccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _logger.LogInformation("AccountService initialized.");
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