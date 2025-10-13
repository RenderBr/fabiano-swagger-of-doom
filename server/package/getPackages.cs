#region

using db;
using db.Repositories;
using db.Services;
using System;
using System.Collections.Generic;
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

namespace server.package
{
    internal class getPackages : RequestHandler
    {
        internal static Dictionary<string, Package> CurrentPackages { get; set; }

        protected override Task HandleRequest()
        {
            string s = Serialize();

            using StreamWriter wtr = new StreamWriter(Context.Response.OutputStream);
            wtr.Write(s);
            
            return Task.CompletedTask;
        }

        internal static string Serialize()
        {
            XmlDocument doc = new XmlDocument();
            XmlNode packageResponse = doc.CreateElement("PackageResponse");
            doc.AppendChild(packageResponse);

            XmlNode packages = doc.CreateElement("Packages");
            packageResponse.AppendChild(packages);

            using (var scope = Program.Services.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var activePackages = unitOfWork.Context.Packages.Where(p => p.EndDate >= DateTime.UtcNow).ToList();

                foreach (var pkg in activePackages)
                {
                    XmlNode packageElem = doc.CreateElement("Package");
                    XmlAttribute packageElemId = doc.CreateAttribute("id");
                    packageElemId.Value = pkg.Id.ToString();
                    packageElem.Attributes.Append(packageElemId);

                    XmlNode name = doc.CreateElement("Name");
                    name.InnerText = pkg.Name;
                    packageElem.AppendChild(name);

                    XmlNode price = doc.CreateElement("Price");
                    price.InnerText = pkg.Price.ToString();
                    packageElem.AppendChild(price);

                    XmlNode quantity = doc.CreateElement("Quantity");
                    quantity.InnerText = pkg.Quantity.ToString();
                    packageElem.AppendChild(quantity);

                    XmlNode maxPurchase = doc.CreateElement("MaxPurchase");
                    maxPurchase.InnerText = pkg.MaxPurchase.ToString();
                    packageElem.AppendChild(maxPurchase);

                    XmlNode weight = doc.CreateElement("Weight");
                    weight.InnerText = pkg.Weight.ToString();
                    packageElem.AppendChild(weight);

                    XmlNode bgUrl = doc.CreateElement("BgURL");
                    bgUrl.InnerText = pkg.BgUrl;
                    packageElem.AppendChild(bgUrl);

                    XmlNode endDate = doc.CreateElement("EndDate");
                    DateTime dt = pkg.EndDate.Kind != DateTimeKind.Utc ?
                        pkg.EndDate.ToUniversalTime() : pkg.EndDate;
                    endDate.InnerText = String.Format("{0}/{1}/{2} {3} GMT-0000", dt.Day, dt.Month, dt.Year, dt.ToLongTimeString());
                    packageElem.AppendChild(endDate);
                    packages.AppendChild(packageElem);
                }
            }

            StringWriter wtr = new StringWriter();
            doc.Save(wtr);
            return wtr.ToString();
        }
    }

    public class Package
    {
        public int PackageId { get; set; }

        public string Name { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int MaxPurchase { get; set; }
        public int Weight { get; set; }
        public string BgURL { get; set; }
        public DateTime EndDate { get; set; }
        public string Contents { get; set; }

        internal static Package GetPackage(int id)
        {
            using (var scope = Program.Services.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var pkg = unitOfWork.Context.Packages.FirstOrDefault(p => p.Id == id && p.EndDate >= DateTime.UtcNow);
                if (pkg == null) return null;

                return new Package
                {
                    BgURL = pkg.BgUrl,
                    EndDate = pkg.EndDate,
                    Weight = pkg.Weight,
                    MaxPurchase = pkg.MaxPurchase,
                    Name = pkg.Name,
                    PackageId = pkg.Id,
                    Price = pkg.Price,
                    Quantity = pkg.Quantity,
                    Contents = pkg.Contents
                };
            }
        }
    }
}