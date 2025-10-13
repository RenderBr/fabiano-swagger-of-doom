using System.IO;
using System.Threading.Tasks;
using db.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace server.account
{
    internal class resetPassword : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var account = await accountRepository.GetByAuthTokenAsync(Query["authToken"]);
            if (account != null)
            {
                string password = Utils.GenerateRandomString(10);
                account.Password = Utils.Sha1(password);
                await accountRepository.SaveChangesAsync();

                using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                    wtr.Write(resetPasswordSuccess.Replace("{PASSWORD}", password).Replace("{SERVERDOMAIN}", Program.Config.ServerDomain));
            }
            else
            {
                using (StreamWriter wtr = new StreamWriter(Context.Response.OutputStream))
                    wtr.Write(resetPasswordFailure);
            }
        }

        private const string resetPasswordSuccess =
@"<html>
<body bgcolor=""#000000"">
    <div align=""center"">
        <font color=""#FFFFFF"">Your new password is {PASSWORD}, please note that passwords are CaSeSensItivE. Play the game <a href=""{SERVERDOMAIN}"">here</a>.</font>
    </div>
</body>
</html>";

        private const string resetPasswordFailure =
@"<html>
<body bgcolor=""#000000"">
    <div align=""center"">
        <font color=""#FFFFFF"">Ohhh something went wrong, please request a new password.</font>
    </div>
</body>
</html>";
    }
}
