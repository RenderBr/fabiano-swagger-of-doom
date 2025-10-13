#region

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

namespace server.@char
{
    internal class delete : RequestHandler
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
                var character = await unitOfWork.Characters.GetByCharacterIdAsync(acc.Id, int.Parse(Query["charId"]));
                if (character != null)
                {
                    await unitOfWork.Characters.DeleteAsync(character);
                    status = Encoding.UTF8.GetBytes("<Success />");
                }
                else
                {
                    status = Encoding.UTF8.GetBytes("<Error>Character not found</Error>");
                }
            }
            Context.Response.OutputStream.Write(status, 0, status.Length);
        }
    }
}