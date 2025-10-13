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
    internal class setBoard : RequestHandler
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
                    // Set the board for the account's guild
                    var board = await unitOfWork.Context.Boards.FindAsync(acc.GuildId);
                    if (board == null)
                    {
                        board = new Board { GuildId = acc.GuildId, Text = Query["board"] };
                        unitOfWork.Context.Boards.Add(board);
                    }
                    else
                    {
                        board.Text = Query["board"];
                        unitOfWork.Context.Boards.Update(board);
                    }
                    await unitOfWork.SaveChangesAsync();
                    status = Encoding.UTF8.GetBytes("<Success />");
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