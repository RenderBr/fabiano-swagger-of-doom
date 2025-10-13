#region

using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using db;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.guild
{
    internal class getBoard : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            byte[] status = new byte[0];
            if (acc != null)
            {
                try
                {
                    // Get the board for the account's guild
                    var board = await unitOfWork.Context.Boards.FindAsync(acc.GuildId);
                    string boardText = board?.Text ?? "";
                    status = Encoding.UTF8.GetBytes(boardText);
                }
                catch (Exception e)
                {
                    status = Encoding.UTF8.GetBytes("<Error>" + e.Message + "</Error>");
                }
            }
            Context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}