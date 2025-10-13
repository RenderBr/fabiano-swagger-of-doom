#region

using db;
using db.Repositories;
using db.Services;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.fame
{
    internal class list : RequestHandler
    {
        protected async override Task HandleRequest()
        {
            byte[] status = null;

            string span = "";
            switch (Query["timespan"])
            {
                case "week":
                    span = "(time >= DATE_SUB(NOW(), INTERVAL 1 WEEK))";
                    break;
                case "month":
                    span = "(time >= DATE_SUB(NOW(), INTERVAL 1 MONTH))";
                    break;
                case "all":
                    span = "TRUE";
                    break;
                default:
                    status = Encoding.UTF8.GetBytes("<Error>Invalid fame list</Error>");
                    break;
            }
            string ac = "FALSE";
            if (Query["accountId"] != null)
                ac = "(accId=@accId AND chrId=@charId)";

            if (status == null)
            {
                XmlDocument doc = new XmlDocument();
                XmlElement root = doc.CreateElement("FameList");

                XmlAttribute spanAttr = doc.CreateAttribute("timespan");
                spanAttr.Value = Query["timespan"];
                root.Attributes.Append(spanAttr);

                doc.AppendChild(root);
                
                using (var scope = Program.Services.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

                    // Use EF to query deaths
                    var deathsQuery = unitOfWork.Context.Deaths.AsQueryable();
                    if (Query["timespan"] == "week")
                        deathsQuery = deathsQuery.Where(d => d.Time >= DateTime.Now.AddDays(-7));
                    else if (Query["timespan"] == "month")
                        deathsQuery = deathsQuery.Where(d => d.Time >= DateTime.Now.AddMonths(-1));
                    // for "all", no filter

                    if (Query["accountId"] != null)
                    {
                        long accId = long.Parse(Query["accountId"]);
                        int chrId = int.Parse(Query["charId"]);
                        deathsQuery = deathsQuery.Where(d => d.AccountId == accId && d.CharacterId == chrId);
                    }

                    var deaths = await deathsQuery.OrderByDescending(d => d.TotalFame).Take(20).ToListAsync();

                    foreach (var death in deaths)
                    {
                        XmlElement elem = doc.CreateElement("FameListElem");

                        XmlAttribute accIdAttr = doc.CreateAttribute("accountId");
                        accIdAttr.Value = death.AccountId.ToString();
                        elem.Attributes.Append(accIdAttr);
                        XmlAttribute chrIdAttr = doc.CreateAttribute("charId");
                        chrIdAttr.Value = death.CharacterId.ToString();
                        elem.Attributes.Append(chrIdAttr);

                        root.AppendChild(elem);

                        XmlElement nameElem = doc.CreateElement("Name");
                        nameElem.InnerText = String.Empty;
                        elem.AppendChild(nameElem);
                        XmlElement objTypeElem = doc.CreateElement("ObjectType");
                        objTypeElem.InnerText = death.CharacterType.ToString();
                        elem.AppendChild(objTypeElem);
                        XmlElement tex1Elem = doc.CreateElement("Tex1");
                        tex1Elem.InnerText = death.Tex1.ToString();
                        elem.AppendChild(tex1Elem);
                        XmlElement tex2Elem = doc.CreateElement("Tex2");
                        tex2Elem.InnerText = death.Tex2.ToString();
                        elem.AppendChild(tex2Elem);
                        XmlElement skinElem = doc.CreateElement("Texture");
                        skinElem.InnerText = death.Skin.ToString();
                        elem.AppendChild(skinElem);
                        XmlElement equElem = doc.CreateElement("Equipment");
                        equElem.InnerText = death.Items;
                        elem.AppendChild(equElem);
                        XmlElement fameElem = doc.CreateElement("TotalFame");
                        fameElem.InnerText = death.TotalFame.ToString();
                        elem.AppendChild(fameElem);
                    }

                    XmlNodeList list = doc.SelectNodes("/FameList/FameListElem");

                    foreach (XmlNode node in list)
                    {
                        foreach (XmlNode xnode in node.ChildNodes)
                        {
                            if (xnode.Name == "Name")
                            {
                                long accId = long.Parse(node.Attributes["accountId"].Value);
                                var account = await accountRepository.GetByIdAsync(accId);
                                xnode.InnerText = account?.Name ?? "";
                            }
                        }
                    }
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = true;
                using (XmlWriter wtr = XmlWriter.Create(Context.Response.OutputStream))
                    doc.Save(wtr);
            }
            else
            {
                Context.Response.OutputStream.Write(status, 0, status.Length);
            }
        }
    }
}
