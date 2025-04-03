using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string to, string subject, string body)
        {
            var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
            {
                Port = int.Parse(_configuration["Email:Port"]),
                Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:From"]),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);
            smtpClient.Send(mailMessage);
        }

        public void SendVerificationEmail(string userEmail, string verificationToken)
        {
            string verificationLink = $"https://yourwebsite.com/verify?token={verificationToken}";

            string emailBody = $@"
        <h2>Bekräfta din e-postadress</h2>
        <p>Klicka på länken nedan för att bekräfta din e-post:</p>
        <a href='{verificationLink}'>Verifiera mitt konto</a>";

            SendEmail(userEmail, "Bekräfta din e-postadress", emailBody);
        }
    }

}
