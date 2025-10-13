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
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.@char
{
    internal class fame : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            var scope = Program.Services.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var acc = await unitOfWork.Accounts.GetByIdAsync(long.Parse(Query["accountId"]));
            var chr = await unitOfWork.Characters.GetByCharacterIdAsync(acc.Id, int.Parse(Query["charId"]));

            if (chr != null)
            {
                var death = await unitOfWork.Deaths.GetByAccountIdAndCharacterIdAsync(acc.Id, chr.CharacterId);

                if (death != null)
                {
                    int time = (int)(death.Time - new DateTime(1970, 1, 1)).TotalSeconds;
                    string killer = death.Killer;
                    bool firstBorn = death.FirstBorn;

                    await using StreamWriter wtr = new StreamWriter(Context.Response.OutputStream);
                    wtr.Write(chr.FameStats); // Simplified: output the fame stats
                }
            }
        }
    }
}