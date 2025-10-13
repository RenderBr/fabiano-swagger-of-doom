#region

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using server.mysterybox;
using System.Xml;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class purchaseMysteryBox : RequestHandler
    {
        //Thanks to Liinkii for purchasing me a MysteryBox
        //<Success><Awards>ITEM ID</Awards><Gold>GOLD LEFT</Gold></Success>
        private Random rand;

        protected override async Task HandleRequest()
        {
            var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            rand = Query["ignore"] != null ? new Random(int.Parse(Query["ignore"])) : new Random();

            var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            if (acc != null)
            {
                if (Query["boxId"] == null)
                {
                    using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                        wtr.WriteLine("<Error>Box not found</Error>");
                    return;
                }
                server.mysterybox.MysteryBox box = server.mysterybox.MysteryBox.GetBox(int.Parse(Query["boxId"]));
                if (box == null)
                {
                    using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                        wtr.WriteLine("<Error>Box not found</Error>");
                    return;
                }
                int price = 0;
                int currency = 0;
                if (box.Sale != null && DateTime.UtcNow <= box.Sale.SaleEnd)
                {
                    price = box.Sale.Price;
                    currency = box.Sale.Currency;
                }
                else
                {
                    price = box.Price.Amount;
                    currency = box.Price.Currency;
                }

                if (currency == 0 && acc.Stats.Credits < price)
                {
                    using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                        wtr.WriteLine("<Error>Not Enough Gold</Error>");
                    return;
                }
                if (currency == 1 && acc.Stats.Fame < price)
                {
                    using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                        wtr.WriteLine("<Error>Not Enough Fame</Error>");
                    return;
                }

                MysteryBoxResult res = new MysteryBoxResult
                {
                    Awards = Utils.GetCommaSepString(GetAwards(box.Contents))
                };

                if (currency == 0)
                {
                    acc.Stats.Credits -= price;
                    res.GoldLeft = acc.Stats.Credits;
                }
                else
                {
                    acc.Stats.Fame -= price;
                    res.GoldLeft = acc.Stats.Fame;
                }
                res.Currency = currency;

                sendMysteryBoxResult(Context.Response.OutputStream, res);

                int[] gifts = Utils.FromCommaSepString32(res.Awards);
                var giftList = Utils.FromCommaSepString32(acc.Gifts).ToList();
                foreach (int item in gifts)
                    giftList.Add(item);
                acc.Gifts = Utils.GetCommaSepString(giftList.ToArray());

                await accountRepository.SaveChangesAsync();
            }
            else
                using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                    wtr.WriteLine("<Error>Account not found</Error>");
        }

        private int[] GetAwards(string items)
        {
            int[] ret = new int[items.Split(';').Length];
            for (int i = 0; i < ret.Length; i++)
                ret[i] = Utils.FromString(items.Split(';')[0].Split(',')[rand.Next(items.Split(';')[0].Split(',').Length)]);
            return ret.ToArray();
        }

        private void sendMysteryBoxResult(Stream stream, MysteryBoxResult res)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode success = doc.CreateElement("Success");
            doc.AppendChild(success);
            
            XmlNode awards = doc.CreateElement("Awards");
            awards.InnerText = res.Awards.Replace(" ", String.Empty);
            success.AppendChild(awards);


            XmlNode goldLeft = doc.CreateElement(res.Currency == 0 ? "Gold" : "Fame");
            goldLeft.InnerText = res.GoldLeft.ToString();
            success.AppendChild(goldLeft);

            StringWriter wtr = new StringWriter();
            doc.Save(wtr);
            using (StreamWriter output = new StreamWriter(stream))
                output.WriteLine(wtr.ToString());
        }

        private class MysteryBoxResult
        {
            public string Awards { get; set; }
            public int GoldLeft { get; set; }
            public int Currency { get; set; }
        }
    }
}