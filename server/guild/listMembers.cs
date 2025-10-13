#region

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using db;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.guild
{
    internal class listMembers : RequestHandler
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
                    int num = Convert.ToInt32(Query["num"]);
                    int offset = Convert.ToInt32(Query["offset"]);

                    // Get guild members
                    var members = unitOfWork.Context.Accounts
                        .Where(a => a.GuildId == acc.GuildId)
                        .OrderBy(a => a.Id)
                        .Skip(offset)
                        .Take(num)
                        .ToList();

                    // Build XML response
                    var xml = new StringBuilder();
                    xml.Append("<Guild>");
                    foreach (var member in members)
                    {
                        xml.Append("<Member>");
                        xml.Append($"<Name>{member.Name}</Name>");
                        xml.Append($"<Rank>{member.GuildRank}</Rank>");
                        xml.Append($"<Fame>{member.GuildFame}</Fame>");
                        xml.Append("</Member>");
                    }
                    xml.Append("</Guild>");

                    status = Encoding.UTF8.GetBytes(xml.ToString());
                }
                catch
                {
                    status = Encoding.UTF8.GetBytes("<Error>Guild member error</Error>");
                }
            }
            Context.Response.OutputStream.Write(status, 0, status.Length);
            Context.Response.Close();
        }
    }
}