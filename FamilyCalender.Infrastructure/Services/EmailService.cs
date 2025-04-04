using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FamilyCalender.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FamilyCalender.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
			_httpContextAccessor = httpContextAccessor;
		}

	private void SendEmail(string to, string subject, string body)
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
			var request = _httpContextAccessor.HttpContext?.Request;
			var verificationLink = $"{request?.Scheme}://{request?.Host}/VerifyAccount/{verificationToken}";



			string emailBody = $@"
        <h2>Bekräfta din e-postadress</h2>
        <p>Klicka på länken nedan för att bekräfta din e-post:</p>
        <a href='{verificationLink}'>Verifiera mitt konto</a>";

            SendEmail(userEmail, "Bekräfta din e-postadress", emailBody);
        }

        public void SendPasswordResetEmail(string userEmail, string resetToken)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            var resetLink = $"{request?.Scheme}://{request?.Host}/ResetPassword/{resetToken}";

            string emailBody = $@"
        <h2>Återställ ditt lösenord</h2>
        <p>Klicka på länken nedan för att återställa ditt lösenord:</p>
        <a href='{resetLink}'>Återställ lösenord</a>";

            SendEmail(userEmail, "Återställ ditt lösenord", emailBody);
        }
    }

}
