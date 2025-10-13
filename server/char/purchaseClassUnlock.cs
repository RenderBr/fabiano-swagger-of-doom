#region

using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using db;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.@char
{
    internal class purchaseClassUnlock : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            try
            {
                var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);

                string classType = Program.GameDataService.ObjectTypeToId[ushort.Parse(Query["classType"])];

                if (acc != null)
                {
                    // TODO: Implement class unlock logic
                    // For now, return error
                    using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                        wtr.Write("<Error>Class unlock not available</Error>");
                    return;
                }
            }
            catch (Exception e)
            {
                using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                {
                    wtr.WriteLine("<Error>Invalid classType");
                    wtr.Flush();
                    wtr.WriteLine(e);
                }
            }
        }
    }
}