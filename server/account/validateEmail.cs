using System.Threading.Tasks;
using db.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace server.account
{
    internal class validateEmail : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var account = await accountRepository.GetByAuthTokenAsync(Query["authToken"]);
            if (account != null)
            {
                account.Verified = true;
                await accountRepository.SaveChangesAsync();
                await Program.SendFileAsync("game/verifySuccess.html", Context);
            }
            else
                await Program.SendFileAsync("game/verifyFail.html", Context);
        }
    }
}
