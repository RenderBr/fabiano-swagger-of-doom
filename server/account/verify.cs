#region

using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class verify : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

            var acc = await accountService.VerifyAsync(Query["guid"], Query["password"]);
            if (await CheckAccount(acc))
            {
                XmlSerializer serializer = new XmlSerializer(acc.GetType(),
                    new XmlRootAttribute(acc.GetType().Name) {Namespace = ""});

                XmlWriterSettings xws = new XmlWriterSettings();
                xws.Indent = true;
                xws.OmitXmlDeclaration = true;
                xws.Encoding = Encoding.UTF8;
                XmlWriter xtw = XmlWriter.Create(Context.Response.OutputStream, xws);
                serializer.Serialize(xtw, acc, null);
            }
        }
    }
}