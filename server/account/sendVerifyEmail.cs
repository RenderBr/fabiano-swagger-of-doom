#region

using System.Threading.Tasks;

#endregion

namespace server.account
{
    internal class sendVerifyEmail : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            await Task.CompletedTask;
        }
    }
}