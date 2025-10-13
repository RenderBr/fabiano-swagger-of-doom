using System.Net.Mail;
using System.Threading.Tasks;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

namespace server.account
{
    internal class changeEmail : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();

            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();
            if (await accountService.VerifyAsync(Query["guid"], Query["password"]) == null)
            {
                WriteErrorLine("Account credentials not valid");
                return;
            }

            var account = await accountRepository.GetByUuidAsync(Query["guid"]);
            if (account == null)
            {
                WriteErrorLine("Account not found");
                return;
            }

            // do not use Database, generate a new random key
            var authKey = Utils.GenerateRandomString(128);

            account.Uuid = Query["newGuid"];
            account.AuthToken = authKey;
            await accountRepository.SaveChangesAsync();

            MailMessage message = new MailMessage();
            message.To.Add(Query["newGuid"]);
            message.IsBodyHtml = true;
            message.Subject = "Please verify your account.";
            message.From = new MailAddress(Program.Config.Smtp.Email);
            message.Body = "<center>Please verify your email via this <a href=\"" + Program.Config.ServerDomain +
                           "/account/validateEmail?authToken=" + authKey +
                           "\" target=\"_blank\">link</a>.</center>";
            await Program.SendEmailAsync(message, true);
        }
    }
}