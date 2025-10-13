#region

using System.Threading.Tasks;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class changePassword : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var account = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            
            if (account == null)
            {
                WriteErrorLine("Account not found");
                return;
            }
            
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
            
            var newPassword = Utils.Sha1(Query["newPassword"]);
            
            if (newPassword == account.Password)
            {
                WriteErrorLine("New password must be different from the old one");
                return;
            }
            
            account.Password = newPassword;
            await accountRepository.SaveChangesAsync();
            WriteLine("<Success />");
        }
    }
}