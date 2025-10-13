#region

using System.Threading.Tasks;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class verifyage : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            if (acc != null)
            {
                acc.IsAgeVerified = true; // Assuming Query["isAgeVerified"] is "1" or something, but probably just set to true
                await accountRepository.SaveChangesAsync();
                WriteLine("<Success/>");
            }
            else
                WriteErrorLine("Error.accountNotFound");
        }
    }
}