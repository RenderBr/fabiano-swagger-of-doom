#region

using System.Threading.Tasks;
using System.Text.RegularExpressions;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class setName : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            if (await CheckAccount(acc))
            {
                if (!acc.NameChosen)
                {
                    if (Regex.IsMatch(Query["name"], @"^[a-zA-Z]+$"))
                    {
                        var existingName = await accountRepository.FirstOrDefaultAsync(a => a.Name == Query["name"]);
                        if (existingName != null)
                            WriteErrorLine("Duplicated name");
                        else
                        {
                            acc.Name = Query["name"];
                            acc.NameChosen = true;
                            await accountRepository.SaveChangesAsync();
                            WriteLine("<Success />");
                        }
                    }
                    else
                        WriteErrorLine("Invalid name");
                }
                else
                    WriteErrorLine("Name already chosen");
            }
        }
    }
}
