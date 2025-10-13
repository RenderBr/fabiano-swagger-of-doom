#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using db.Services;
using System.Net.Sockets;
using db.Models;
using db.Models.Xml;
using db.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RageRealm.Shared.Configuration.WebServer;
using RageRealm.Shared.Utilities;

#endregion

namespace server.@char
{
    internal class list : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            await using var scope = Program.Services.CreateAsyncScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

            Account account = null;
            if (!string.IsNullOrWhiteSpace(Query["guid"]) && !string.IsNullOrWhiteSpace(Query["password"]))
            {
                account = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            }

            var newsService = scope.ServiceProvider.GetRequiredService<INewsRepository>();
            var newsItems = (await newsService.GetLatestAsync(5)).ConvertAll(n => new NewsItem
            {
                Icon = n.Icon,
                Title = n.Title,
                Link = n.Link,
                TagLine = n.Text,
                Date = n.Date.ToUnixTimestamp()
            });
            
            var characterRepository = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
            var characters = account != null
                ? await characterRepository.GetLiveCharactersByAccountIdAsync(account.Id)
                : [];
            
            var chrs = new Chars
            {
                Characters = [],
                NextCharId = 2,
                MaxNumChars = 1,
                Account = account != null ? new AccountXml(account) : null,
                Servers = await GetServerList()
            };

            if (chrs.Account != null && account != null)
            {
                chrs.Characters = characters.Select(Char.FromCharacter).ToList();
                chrs.News = newsItems;
                chrs.OwnedSkins = chrs.OwnedSkins;
                chrs.TOSPopup = account.AcceptedNewTos ? "true" : "false";
            }
            else
            {
                chrs.Account = new AccountXml(AccountUtilities.CreateGuestAccount(Query["guid"] ?? ""));
                chrs.News = newsItems;
            }

            chrs.ClassAvailabilityList = await GetClassAvailability(account);

            var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            string xml;

            var serializer = new XmlSerializer(chrs.GetType(),
                new XmlRootAttribute(chrs.GetType().Name) { Namespace = "" });

            // StringWriter uses StringBuilder, so just keep it sync and ensure full flush
            using (var stringWriter = new StringWriter())
            {
                using (var xw = XmlWriter.Create(stringWriter, new XmlWriterSettings
                       {
                           OmitXmlDeclaration = true,
                           Encoding = encoding,
                           Indent = true,
                           Async = false // keep synchronous to guarantee completion
                       }))
                {
                    serializer.Serialize(xw, chrs, chrs.Namespaces);
                    xw.Flush(); // ✅ ensure XmlWriter writes all buffered text
                }

                xml = stringWriter.ToString();
            }

            // convert to bytes and write with explicit content-length
            byte[] buffer = encoding.GetBytes(xml);
            Context.Response.ContentType = "text/xml; charset=utf-8";
            Context.Response.ContentLength64 = buffer.Length; // disables chunked transfer

            await Context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            await Context.Response.OutputStream.FlushAsync();
            Context.Response.Close(); // closes TCP socket cleanly

        }

        private static async Task<List<ServerItem>> GetServerList()
        {
            var ret = new List<ServerItem>();

            var tasks = new List<Task<(ServerItemConfiguration server, double usage)>>();

            foreach (var server in Program.Config.Servers)
            {
                tasks.Add(Task.Run(async () => (server, await GetUsage(server.Address))));
            }

            var results = await Task.WhenAll(tasks);

            foreach (var (server, usage) in results)
            {
                ret.Add(new ServerItem
                {
                    Name = server.Name,
                    Lat = 0,
                    Long = 0,
                    DNS = server.Address,
                    Usage = usage,
                    AdminOnly = server.AdminOnly
                });
            }

            return ret;
        }

        private async Task<List<ClassAvailabilityItem>> GetClassAvailability(Account acc)
        {
            var classes = new string[14]
            {
                "Rogue",
                "Assassin",
                "Huntress",
                "Mystic",
                "Trickster",
                "Sorcerer",
                "Ninja",
                "Archer",
                "Wizard",
                "Priest",
                "Necromancer",
                "Warrior",
                "Knight",
                "Paladin"
            };

            if (acc == null)
            {
                return
                [
                    new() { Class = "Rogue", Restricted = "restricted" },
                    new() { Class = "Assassin", Restricted = "restricted" },
                    new() { Class = "Huntress", Restricted = "restricted" },
                    new() { Class = "Mystic", Restricted = "restricted" },
                    new() { Class = "Trickster", Restricted = "restricted" },
                    new() { Class = "Sorcerer", Restricted = "restricted" },
                    new() { Class = "Ninja", Restricted = "restricted" },
                    new() { Class = "Archer", Restricted = "restricted" },
                    new() { Class = "Wizard", Restricted = "unrestricted" },
                    new() { Class = "Priest", Restricted = "restricted" },
                    new() { Class = "Necromancer", Restricted = "restricted" },
                    new() { Class = "Warrior", Restricted = "restricted" },
                    new() { Class = "Knight", Restricted = "restricted" },
                    new() { Class = "Paladin", Restricted = "restricted" }
                ];
            }

            List<ClassAvailabilityItem> ret = new List<ClassAvailabilityItem>();
            
            await using var scope = Program.Services.CreateAsyncScope();
            var unlockedClassRepository = scope.ServiceProvider.GetRequiredService<IUnlockedClassRepository>();
            
            var unlockedClasses = await unlockedClassRepository.GetUserClassesAsync(acc.Id);
            
            if (!unlockedClasses.Any())
            {
                var seededClasses = new UnlockedClass[classes.Length];
                foreach (var uc in unlockedClasses)
                {
                    var index = Array.IndexOf(classes, uc.Class);
                    if (index >= 0)
                    {
                        seededClasses[index] = uc;
                    }
                }
                
                foreach (var uc in seededClasses)
                {
                    if (uc != null)
                    {
                        ret.Add(new ClassAvailabilityItem
                        {
                            Class = uc.Class,
                            Restricted = uc.Available
                        });
                    }
                    else
                    {
                        var className = classes[ret.Count];
                        ret.Add(new ClassAvailabilityItem
                        {
                            Class = className,
                            Restricted = className == "Wizard" ? "unrestricted" : "restricted"
                        });
                    }
                }

                return ret;
            }
            else
            {
                foreach (var uc in unlockedClasses)
                {
                    ret.Add(new ClassAvailabilityItem
                    {
                        Class = uc.Class,
                        Restricted = uc.Available
                    });
                }

                return ret;
            }

            return ret;
        }

        private static async Task<double> GetUsage(string host, int port = 2050)
        {
            using var cli = new TcpClient();

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

                await cli.ConnectAsync(host, port, cts.Token);

                await using var stream = cli.GetStream();

                byte[] request = [0x4d, 0x61, 0x64, 0x65, 0xff];
                await stream.WriteAsync(request, 0, request.Length, cts.Token);

                var buffer = new byte[256];
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token);

                if (bytesRead <= 0)
                    throw new IOException("No data received from server.");

                var s = Encoding.ASCII.GetString(buffer, 0, bytesRead).Split(':');
                if (s.Length < 2)
                    throw new FormatException("Invalid server usage response.");

                return double.Parse(s[1]) / double.Parse(s[0]);
            }
            catch (Exception ex)
            {
                Program.Logger.LogError(ex, "Could not establish connection to {Host}:{Port}", host, port);
                return -1;
            }
        }
    }
}