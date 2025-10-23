#region

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

#endregion

namespace server.sfx
{
    internal class Sfx : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            string localPath = Context.Request.Url.LocalPath.TrimStart('/');
            string file = localPath.StartsWith("music", StringComparison.OrdinalIgnoreCase)
                ? Path.Combine("sfx", localPath)
                : localPath;

            string fullPath = Path.GetFullPath(file);

            if (File.Exists(fullPath))
            {
                Context.Response.ContentType = "audio/mpeg";
                Context.Response.ContentLength64 = new FileInfo(fullPath).Length;
                await using var i = File.OpenRead(fullPath);
                await i.CopyToAsync(Context.Response.OutputStream);
            }
            else
            {
                string redirect = "http://realmofthemadgod.appspot.com/" +
                                  (file.Contains("music", StringComparison.OrdinalIgnoreCase)
                                      ? file.Replace("sfx/", "")
                                      : file);
                Context.Response.Redirect(redirect);
            }
        }
    }
}