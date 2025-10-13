#region

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class purchaseSkin : RequestHandler
    {
        private List<ItemCostItem> Prices =>
        [
            new() { Type = "900", Puchasable = 0, Expires = 0, Price = 90000 },
            new() { Type = "902", Puchasable = 0, Expires = 0, Price = 90000 },
            new() { Type = "834", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "835", Puchasable = 1, Expires = 0, Price = 600 },
            new() { Type = "836", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "837", Puchasable = 1, Expires = 0, Price = 600 },
            new() { Type = "838", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "839", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "840", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "841", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "842", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "843", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "844", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "845", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "846", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "847", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "848", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "849", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "850", Puchasable = 0, Expires = 1, Price = 900 },
            new() { Type = "851", Puchasable = 0, Expires = 1, Price = 900 },
            new() { Type = "852", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "853", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "854", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "855", Puchasable = 1, Expires = 0, Price = 900 },
            new() { Type = "856", Puchasable = 0, Expires = 0, Price = 90000 },
            new() { Type = "883", Puchasable = 0, Expires = 0, Price = 90000 }
        ];

        protected override async Task HandleRequest()
        {
            if (Query.AllKeys.Length > 0)
            {
                using var scope = Program.Services.CreateScope();
                var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
                var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

                var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
                if (await CheckAccount(acc))
                {
                    foreach (var item in Prices)
                    {
                        if (item.Type == Query["skinType"] && item.Puchasable == 1)
                        {
                            var skinId = int.Parse(Query["skinType"]);

                            var ownedSkins = Utils.FromCommaSepString32(acc.OwnedSkins).ToList();
                            if (!ownedSkins.Contains(skinId))
                            {
                                ownedSkins.Add(skinId);
                                acc.OwnedSkins = string.Join(",", ownedSkins);
                                acc.Stats.Credits -= item.Price;
                                await accountRepository.SaveChangesAsync();
                                WriteLine("<Success/>");
                                return;
                            }
                        }
                    }

                    WriteErrorLine("Unable to purchase");
                }
            }
        }
    }
}