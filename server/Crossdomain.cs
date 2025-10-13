#region

using System.Net;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace server
{
    internal class crossdomain : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            var status = """
                         <cross-domain-policy>
                         <allow-access-from domain="*"/>
                         </cross-domain-policy>
                         """u8.ToArray();
            Context.Response.ContentType = "application/xml";
            Context.Response.ContentLength64 = status.Length;
            await Context.Response.OutputStream.WriteAsync(status);
        }
    }
}