#region

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using server.package;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

#endregion

namespace server.account
{
    public class purchasePackage : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            if (Query.AllKeys.Length > 0)
            {
                using var scope = Program.Services.CreateScope();
                var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
                var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

                Package package = Package.GetPackage(int.Parse(Query["packageId"]));

                if (package == null)
                {
                    WriteErrorLine("This package is not available any more");
                    return;
                }

                JsonSerializer s = new JsonSerializer();
                var contents = s.Deserialize<PackageContent>(new JsonTextReader(new StringReader(package.Contents)));

                var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);

                if (await CheckAccount(acc))
                {
                    if (acc.Stats.Credits < package.Price)
                    {
                        WriteErrorLine("Not enough gold.");
                        return;
                    }

                    if (contents.items?.Count > 0)
                    {
                        var giftList = Utils.FromCommaSepString32(acc.Gifts).ToList();
                        foreach (var i in contents.items)
                        {
                            giftList.Add(i);
                        }
                                                acc.Gifts = Utils.GetCommaSepString<int>(giftList.ToArray());
                    }

                    if (contents.charSlots > 0)
                    {
                        acc.MaxCharSlot = (byte)(acc.MaxCharSlot + contents.charSlots);
                    }

                    if (contents.vaultChests > 0)
                    {
                        // TODO: Create chests
                    }

                    acc.Stats.Credits -= package.Price;
                    await accountRepository.SaveChangesAsync();
                    WriteLine("<Success/>");
                }
            }
        }

        struct PackageContent
        {
            public List<int> items;
            public int vaultChests;
            public int charSlots;
        }
    }
}