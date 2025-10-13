#region

using System.Threading.Tasks;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class purchaseCharSlot : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            if (await CheckAccount(acc))
            {
                int charSlotPrice = 100; // Assuming fixed price
                if (acc.Stats.Credits < charSlotPrice)
                    WriteErrorLine("Not enough Gold");
                else
                {
                    acc.Stats.Credits -= charSlotPrice;
                    acc.MaxCharSlot += 1;
                    await accountRepository.SaveChangesAsync();
                    WriteLine("<Success/>");
                }
            }
        }
    }
}