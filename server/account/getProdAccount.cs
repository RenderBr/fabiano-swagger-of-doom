using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Serialization;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace server.account
{
    internal class getProdAccount : RequestHandler
    {
        public const string TRANSFERENGINEVERSION = "v1.0 (beta)";

        protected override async Task HandleRequest()
        {
            string status = "<Error>Internal server error</Error>";
            var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            if (acc != null)
            {
                if (acc.Rank < 1)
                {
                    status = "<Error>Only donators can port prod accounts to the private server.</Error>";
                }
                else if (acc.ProdAcc && acc.Rank < 2)
                {
                    status = "<Error>You account is already transfered.</Error>";
                }
                else if (!acc.Banned)
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(String.Format(
                        "http://www.realmofthemadgodhrd.appspot.com/char/list?guid={0}&password={1}", Query["prodGuid"],
                        Query["prodPassword"]));
                    var resp = req.GetResponse();

                    Chars chrs = new Chars();
                    chrs.Characters = new List<Char>();

                    string s;
                    using (StreamReader rdr = new StreamReader(resp.GetResponseStream()))
                        s = rdr.ReadToEnd();

                    s = s.Replace("False", "false").Replace("True", "true");

                    try
                    {
                        XmlSerializer serializer = new XmlSerializer(chrs.GetType(),
                            new XmlRootAttribute("Chars") { Namespace = "" });
                        chrs = (Chars)serializer.Deserialize(new StringReader(s));

                        // TODO: Implement SaveChars using new API
                        // For now, just set ProdAcc to true
                        acc.ProdAcc = true;
                        await accountRepository.SaveChangesAsync();
                        status = "<Success />";
                    }
                    catch (Exception e)
                    {
                        Program.Logger.LogError(e, "Error deserializing prod account characters.");
                    }
                }
                else
                    status = "<Error>Account under Maintenance</Error>";
            }
            else
                status = "<Error>Account credentials not valid</Error>";

            using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                wtr.Write(status);
        }

        private const string body = @"";
    }
}