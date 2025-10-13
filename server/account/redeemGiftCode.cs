using db;
using db.JsonObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

namespace server.account
{
    internal class redeemGiftCode : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            string code = Query["code"];
            Account acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);

            if (acc != null)
            {
                var giftCode = await unitOfWork.GiftCodes.GetByCodeAsync(code);

                if (giftCode == null)
                {
                    Context.Response.Redirect("../InvalidGiftCode.html");
                    return;
                }

                if (await ParseContents(acc, giftCode.Content, unitOfWork))
                {
                    Context.Response.Redirect("../GiftCodeSuccess.html");
                    unitOfWork.GiftCodes.Remove(giftCode);
                    await unitOfWork.SaveChangesAsync();
                }
                else
                    Context.Response.Redirect("../InvalidGiftCode.html");
            }
        }

        private async Task<bool> ParseContents(Account acc, string json, IUnitOfWork unitOfWork)
        {
            try
            {
                var code = db.JsonObjects.GiftCode.FromJson(json);
                if (code == null) return false;

                if (code.Gifts.Count > 0)
                {
                    var giftList = Utils.FromCommaSepString32(acc.Gifts).ToList();
                    foreach (var i in code.Gifts)
                        giftList.Add(i);
                    acc.Gifts = Utils.GetCommaSepString<int>(giftList.ToArray());
                }

                if (code.CharSlots > 0)
                {
                    acc.MaxCharSlot = (byte)(acc.MaxCharSlot + code.CharSlots);
                }

                if (code.VaultChests > 0)
                {
                    // Simplified: assume creating chests means adding vault data
                    // For now, skip or add logic later
                }

                if (code.Gold > 0)
                {
                    acc.Stats.Credits += code.Gold;
                }

                if (code.Fame > 0)
                {
                    acc.Stats.Fame += code.Fame;
                }

                await unitOfWork.Accounts.SaveChangesAsync();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
