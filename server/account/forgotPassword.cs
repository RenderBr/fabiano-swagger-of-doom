#region

using System;
using System.Net.Mail;
using System.Threading.Tasks;
using db.Repositories;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class forgotPassword : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            using var scope = Program.Services.CreateScope();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            var account = await accountRepository.GetByUuidAsync(Query["guid"]);
            if (account != null)
            {
                string authKey = Utils.GenerateRandomString(128);
                account.AuthToken = authKey;
                await accountRepository.SaveChangesAsync();

                MailMessage message = new MailMessage();
                message.To.Add(Query["guid"]);
                message.Subject = "Forgot Password";
                message.From = new MailAddress(Program.Config.Smtp.Email, "Forgot Password");
                message.Body = emailBody.
                    Replace("{RPLINK}", String.Format("{0}/{1}{2}", Program.Config.ServerDomain, "account/resetPassword?authToken=", authKey)).
                    Replace("{SUPPORTLINK}", String.Format("{0}", Program.Config.SupportDomain)).
                    Replace("{SERVERDOMAIN}", Program.Config.ServerDomain);

                await Program.SendEmailAsync(message, true);
            }
            else
                WriteErrorLine("Error.accountNotFound");
        }

        const string emailBody = @"Hello,

If your wish to reset your password in Fabiano Swagger of Doom, please use the 
link below:

{RPLINK}

If you do NOT wish to reset your password, do nothing.

Do not reply to this email, it will not be read. If you need support, go 
here:

{SUPPORTLINK}

- Fabiano Swagger of Doom Team
{SERVERDOMAIN}";
    }
}