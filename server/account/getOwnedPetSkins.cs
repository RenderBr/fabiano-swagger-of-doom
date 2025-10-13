using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace server.account;

public class getOwnedPetSkins : RequestHandler
{
    protected override async Task HandleRequest()
    {
        Context.Response.ContentType = "text/xml";
        var response = "<Success/>";
        await using var writer = new StreamWriter(Context.Response.OutputStream, Encoding.UTF8);
        await writer.WriteAsync(response);
        await writer.FlushAsync();
        await Context.Response.OutputStream.FlushAsync();
        Context.Response.Close();
    }
}