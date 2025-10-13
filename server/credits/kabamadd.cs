using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace server.credits
{
    internal class kabamadd : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
            {
                string s = File.ReadAllText("game/saved_resource.htm");
                wtr.Write(s);
            }
            await Task.CompletedTask;
        }
    }
}
