#region

using System.IO;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace server.app
{
    internal class init : RequestHandler
    {
        private readonly string text = File.ReadAllText("init.txt");

        protected override async Task HandleRequest()
        {
            var buf = Encoding.ASCII.GetBytes(text);
            await Context.Response.OutputStream.WriteAsync(buf, 0, buf.Length);
        }
    }
}