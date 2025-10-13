#region

using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;

#endregion

namespace server.picture
{
    internal class get : RequestHandler
    {
        private readonly byte[] buff = new byte[0x10000];

        protected override Task HandleRequest()
        {
            //warning: maybe has hidden url injection
            string id = Query["id"];

            if (!id.StartsWith("draw:") && !id.StartsWith("file:"))
            {
                string path = Path.GetFullPath("texture/" + id + ".png");
                if (!File.Exists(path))
                {
                    Program.Logger.LogWarning("RemoteTexture not found: {Id}", id);
                    byte[] status = Encoding.UTF8.GetBytes("<Error>Invalid ID.</Error>");
                    Context.Response.OutputStream.Write(status, 0, status.Length);
                    return Task.CompletedTask;
                }
                using (FileStream i = File.OpenRead(path))
                {
                    int c;
                    while ((c = i.Read(buff, 0, buff.Length)) > 0)
                        Context.Response.OutputStream.Write(buff, 0, c);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}