#region

using db;
using db.Repositories;
using db.Services;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.mysterybox
{
    internal class getBoxes : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            string s = MysteryBox.Serialize();

            //<FortuneGame id = "-3" title = "Armor of the Mad God #1" ><Description></Description><Contents>2835,2833,3105,3176,8812,3290,3279,3278,8851,8781,9017,9015,3239,3133,4103,2873,2872,3105,2762,2761,2766,2764,2759,2760,2765,9015,3276,3264,3275,3133,3177,3178,3270,1803,3138,3269,3293,3180,3274,3272</Contents><Price firstInGold="100" firstInToken="1" secondInGold="250"/><Image>http://rotmg.kabamcdn.com/MadGodArmorAlchemistRewards.png</Image><Icon></Icon><StartTime>2014-09-18 13:25:20</StartTime><EndTime>2014-09-23 13:33:00</EndTime></FortuneGame>

            using StreamWriter wtr = new StreamWriter(Context.Response.OutputStream);
            wtr.Write(s);
            
            await Task.CompletedTask;
        }
    }

    internal class MysteryBox
    {
        internal int BoxId { get; set; }
        internal string Title { get; set; }
        internal int Weight { get; set; }
        internal string Description { get; set; }
        internal string Contents { get; set; }
        internal string Image { get; set; }
        internal string Icon { get; set; }
        internal Price Price { get; set; }
        internal DateTime StartTime { get; set; }
        internal Sale Sale { get; set; }

        internal static MysteryBox GetBox(int id)
        {
            using (var scope = Program.Services.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var box = unitOfWork.Context.MysteryBoxes.FirstOrDefault(m => m.Id == id && m.BoxEnd >= DateTime.Now);
                if (box == null) return null;

                return new MysteryBox
                {
                    BoxId = id,
                    Contents = box.Contents,
                    Weight = box.Weight,
                    Title = box.Title,
                    Description = box.Description,
                    Icon = box.Icon,
                    Image = box.Image,
                    StartTime = box.StartTime,
                    Price = new Price
                    {
                        Amount = box.PriceAmount,
                        Currency = box.PriceCurrency
                    },
                    Sale = box.SaleEnd == DateTime.MinValue ? null :
                    new Sale
                    {
                        SaleEnd = box.SaleEnd,
                        Currency = box.SaleCurrency,
                        Price = box.SalePrice
                    }
                };
            }
        }

        internal static string Serialize()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode minigames = doc.CreateElement("MiniGames");
            XmlAttribute minigamesVersion = doc.CreateAttribute("version");
            minigamesVersion.Value = "1402333568.446112";
            minigames.Attributes.Append(minigamesVersion);
            doc.AppendChild(minigames);

            using (var scope = Program.Services.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var activeBoxes = unitOfWork.Context.MysteryBoxes.Where(m => m.BoxEnd >= DateTime.Now).ToList();

                foreach (var box in activeBoxes)
                {
                    XmlNode boxElem = doc.CreateElement("MysteryBox");
                    XmlAttribute boxId = doc.CreateAttribute("id");
                    boxId.Value = box.Id.ToString();
                    XmlAttribute boxTitle = doc.CreateAttribute("title");
                    boxTitle.Value = box.Title;
                    XmlAttribute boxWeight = doc.CreateAttribute("weight");
                    boxWeight.Value = box.Weight.ToString();
                    boxElem.Attributes.Append(boxId);
                    boxElem.Attributes.Append(boxTitle);
                    boxElem.Attributes.Append(boxWeight);

                    XmlNode desc = doc.CreateElement("Description");
                    desc.InnerText = box.Description;
                    boxElem.AppendChild(desc);

                    XmlNode contents = doc.CreateElement("Contents");
                    contents.InnerText = box.Contents;
                    boxElem.AppendChild(contents);

                    XmlNode price = doc.CreateElement("Price");
                    XmlAttribute priceAmount = doc.CreateAttribute("amount");
                    priceAmount.Value = box.PriceAmount.ToString();
                    XmlAttribute priceCurrency = doc.CreateAttribute("currency");
                    priceCurrency.Value = box.PriceCurrency.ToString();
                    price.Attributes.Append(priceAmount);
                    price.Attributes.Append(priceCurrency);
                    boxElem.AppendChild(price);

                    XmlNode image = doc.CreateElement("Image");
                    image.InnerText = box.Image;
                    boxElem.AppendChild(image);

                    XmlNode icon = doc.CreateElement("Icon");
                    icon.InnerText = box.Icon;
                    boxElem.AppendChild(icon);

                    XmlNode startTime = doc.CreateElement("StartTime");
                    startTime.InnerText = box.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    boxElem.AppendChild(startTime);

                    if (box.SaleEnd != DateTime.MinValue)
                    {
                        XmlNode salePrice = doc.CreateElement("Sale");
                        XmlNode saleEnd = doc.CreateElement("End");
                        saleEnd.InnerText = box.SaleEnd.ToString("yyyy-MM-dd HH:mm:ss");
                        XmlAttribute saleAmount = doc.CreateAttribute("price");
                        saleAmount.Value = box.SalePrice.ToString();
                        XmlAttribute saleCurrency = doc.CreateAttribute("currency");
                        saleCurrency.Value = box.SaleCurrency.ToString();
                        salePrice.Attributes.Append(saleAmount);
                        salePrice.Attributes.Append(saleCurrency);
                        salePrice.AppendChild(saleEnd);
                        boxElem.AppendChild(salePrice);
                    }
                    minigames.AppendChild(boxElem);
                }

                // TODO: Migrate theAlchemist part
                // For now, leave it or remove if not needed
            }
            StringWriter wtr = new StringWriter();
            doc.Save(wtr);
            return wtr.ToString();
        }
    }

    internal class Price
    {
        internal int Amount { get; set; }
        internal int Currency { get; set; }
    }

    internal class Sale
    {
        internal int Price { get; set; }
        internal int Currency { get; set; }
        internal DateTime SaleEnd { get; set; }
    }
}