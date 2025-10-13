using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

namespace server.account
{
    internal class playFortuneGame : RequestHandler
    {
        private static Dictionary<string, int[]> CurrentGames = new Dictionary<string, int[]>();

        private const int GOLD = 0;
        private const int FORTUNETOKENS = 2;

        protected override async Task HandleRequest()
        {
            using (var scope = Program.Services.CreateScope())
            {
                var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                int currency = -1;
                int price = -1;
                int.TryParse(Query["currency"], out currency);
                string status = "<Error>Internal Server Error</Error>";
                Account acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);

                if (acc != null)
                {
                    // TODO: Migrate the query to EF
                    // For now, return error
                    status = "<Error>Game not available</Error>";
                }
                else
                    status = "<Error>Account not found</Error>";

                using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                    wtr.Write(status);
            }
        }
    }
}
