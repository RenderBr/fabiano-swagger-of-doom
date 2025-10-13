#region

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Mail;
using db.Models;
using db.Repositories;
using db.Services;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace server.account
{
    internal class register : RequestHandler
    {
        protected override async Task HandleRequest()
        {
            if (Query["ignore"] == null || !String.IsNullOrWhiteSpace(Query["entrytag"]) || !Query["newGUID"].Contains('@'))
            {
                WriteErrorLine("WebRegister.invalid_email_address");
                return;
            }

            if (String.IsNullOrWhiteSpace(Query["isAgeVerified"]))
            {
                WriteErrorLine("WebRegister.invalid_birthdate");
                return;
            }

            if(!IsValidEmail(Query["newGuid"]))
            {
                WriteErrorLine("WebRegister.invalid_email_address");
                return;
            }

            using var scope = Program.Services.CreateScope();
            var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository>();

            if (!IsValidEmail(Query["newGUID"]))
            {
                WriteErrorLine("WebForgotPasswordDialog.emailError");
                return;
            }

            var existingGuidAccount = await accountRepository.GetByUuidAsync(Query["guid"]);
            if (existingGuidAccount != null && !existingGuidAccount.Guest)
            {
                var newGuidAccount = await accountRepository.GetByUuidAsync(Query["newGUID"]);
                if (newGuidAccount != null)
                {
                    WriteErrorLine("Error.emailAlreadyUsed");
                    return;
                }
                else
                {
                    // Upgrade guest account
                    existingGuidAccount.Uuid = Query["newGUID"];
                    existingGuidAccount.Name = Query["name"];
                    existingGuidAccount.Password = Utils.Sha1(Query["newPassword"]);
                    existingGuidAccount.Guest = false;
                    await accountRepository.SaveChangesAsync();
                    WriteLine("<Success />");
                    return;
                }
            }
            else
            {
                // Create new account
                var newAccount = new Account
                {
                    Uuid = Query["newGUID"],
                    Password = Utils.Sha1(Query["newPassword"]),
                    Name = Query["newGUID"],
                    Guest = false,
                    AuthToken = Guid.NewGuid().ToString(),
                    RegTime = DateTime.Now,
                    LastSeen = DateTime.Now
                };

                await accountRepository.AddAsync(newAccount);
                await accountRepository.SaveChangesAsync();

                if (Program.Config.VerifyEmail)
                {
                    MailMessage message = new MailMessage();
                    message.To.Add(Query["newGuid"]);
                    message.IsBodyHtml = true;
                    message.Subject = "Please verify your account.";
                    message.From = new MailAddress(Program.Config.Smtp.Email);
                    message.Body = "<center>Please verify your email via this <a href=\"" + Program.Config.ServerDomain + "/account/validateEmail?authToken=" + newAccount.AuthToken + "\" target=\"_blank\">link</a>.</center>";
                    await Program.SendEmailAsync(message, true);
                }
                WriteLine("<Success/>");
            }
        }

        public bool IsValidEmail(string strIn)
        {
            var invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            MatchEvaluator DomainMapper = match =>
            {
                // IdnMapping class with default property values.
                IdnMapping idn = new IdnMapping();

                string domainName = match.Groups[2].Value;
                try
                {
                    domainName = idn.GetAscii(domainName);
                }
                catch (ArgumentException)
                {
                    invalid = true;
                }
                return match.Groups[1].Value + domainName;
            };

            // Use IdnMapping class to convert Unicode domain names. 
            strIn = Regex.Replace(strIn, @"(@)(.+)$", DomainMapper);
            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            if(Regex.IsMatch(strIn,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                      RegexOptions.IgnoreCase))
            {
                string s = strIn.Remove(0, strIn.IndexOf('@') + 1);
                string emailBody = s.Remove(s.IndexOf('.'));
                switch (emailBody)
                {
                    case "mail":
                    case "yandex":
                    case "yahoo":
                    case "aol":
                    case "eclipso":
                    case "maills":
                    case "gmail":
                    case "googlemail":
                    case "firemail":
                    case "maili":
                    case "hotmail":
                    case "emailn":
                    case "outlook":
                    case "rediffmail":
                    case "oyoony":
                    case "lycos":
                    case "directbox":
                    case "new-post":
                    case "gmx":
                    case "slucia":
                    case "5x2":
                    case "smart-mail":
                    case "spl":
                    case "t-online":
                    case "compu-freemail":
                    case "web":
                    case "x-mail":
                    case "k":
                    case "mc-free":
                    case "freenet":
                    case "k-bg":
                    case "overmail":
                    case "anpa":
                    case "freemailer":
                    case "vcmail":
                    case "mail4nature":
                    case "uims":
                    case "1vbb":
                    case "uni":
                    case "techmail":
                    case "hushmail":
                    case "freemail-24":
                    case "guru":
                    case "email":
                    case "1email":
                    case "canineworld":
                    case "zelx":
                    case "sify":
                    case "softhome":
                    case "kuekomail":
                    case "mailde":
                    case "mail-king":
                    case "noxamail":
                    case "h3c":
                    case "arcor":
                    case "logomail":
                    case "ueberschuss":
                    case "chattler":
                    case "modellraketen":
                        return true;
                }
            }
            return false;
        }
    }
}