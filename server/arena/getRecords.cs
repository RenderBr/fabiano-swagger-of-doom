#region

using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using db;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.Arena
{
    internal class getRecords : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            string result = "";
            using (var scope = Program.Services.CreateScope())
            {
                var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                Account acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
                if (String.IsNullOrEmpty(Query["guid"]) ||
                    String.IsNullOrEmpty(Query["password"]) ||
                    String.IsNullOrEmpty(Query["type"]) ||
                    acc == null)
                {
                    Context.Response.StatusCode = 400;
                    result = "<Error>Invalid GUID/password combination</Error>";
                }
                else
                {
                    // TODO: Implement GetArenaLeaderboards via repository
                    string[][] ranks = new string[0][]; // Placeholder, need to implement
                    result += "<ArenaRecords>";
                    foreach (string[] i in ranks)
                    {
                        // Get character data
                        var character = await unitOfWork.Characters.GetByCharacterIdAsync(acc.Id, int.Parse(i[2]));
                        string skin = character?.Skin.ToString() ?? "0";
                        string tex1 = character?.Tex1.ToString() ?? "0";
                        string tex2 = character?.Tex2.ToString() ?? "0";
                        string inventory = string.Join(",", character?.Items ?? []);
                        string cclass = character?.CharacterType.ToString() ?? "0";

                        result += "<Record>";
                        //wave number
                        result += "<WaveNumber>" + i[0] + "</WaveNumber>";
                        //playtime
                        result += "<Time>" + i[4] + "</Time>";
                        result += "<PlayData>";
                        result += "<CharacterData>";
                        result += "<GuildName></GuildName>"; // Placeholder
                        result += "<GuildRank>0</GuildRank>"; // Placeholder
                        result += "<Id>" + i[2] + "</Id>";
                        result += "<Texture>" + skin + "</Texture>";
                        result += "<Inventory>" + inventory + "</Inventory>";
                        result += "<Name>" + acc.Name + "</Name>";
                        result += "<Class>" + cclass + "</Class>";
                        result += "</CharacterData>";
                        result += "<Pet name=\"" +
                                  "\" type=\"" +
                                  "\" instanceId=\"" +
                                  "\" rarity=\"" +
                                  "\" maxAbilityPower=\"" +
                                  "\" skin=\"" +
                                  "\" family=\"" +
                                  "\">";
                        result += "<Abilities>";
                        result += "</Abilities>";
                        result += "</Pet>";
                        result += "</PlayData>";
                        result += "</Record>";
                    }
                    result += "</ArenaRecords>";
                }
            }
            byte[] buf = Encoding.UTF8.GetBytes(result);
            Context.Response.OutputStream.Write(buf, 0, buf.Length);
        }
    }
}