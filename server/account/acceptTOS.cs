using System.Threading.Tasks;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

namespace server.account
{
    internal class acceptTOS : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var account = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            if (account == null)
            {
                WriteErrorLine("Account not found");
                return;
            }

            if (account.AcceptedNewTos)
            {
                WriteErrorLine("TOS Already Accepted");
                return;
            }

            account.AcceptedNewTos = true;
            await accountRepository.SaveChangesAsync();
            WriteLine("<Success/>");
        }
    }
}