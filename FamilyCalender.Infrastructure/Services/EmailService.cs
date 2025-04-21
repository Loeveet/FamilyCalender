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
		//private readonly IConfiguration _configuration;
		// private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly EmailSettings _emailSettings;

		public EmailService(EmailSettings emailSettings)
		{
			_emailSettings = emailSettings;
		}
		//     public EmailService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
		//     {
		//         _configuration = configuration;
		//_httpContextAccessor = httpContextAccessor;
		//         _emailSettings = configuration.GetSection("Email").Get<EmailSettings>() ?? new EmailSettings();
		//     }

		private void SendEmail(string to, string subject, string body)
		{
			var smtpClient = new SmtpClient(_emailSettings.SmtpServer)
			{
				Port = _emailSettings.Port,
				Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
				EnableSsl = true
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress(_emailSettings.From),
				Subject = subject,
				Body = body,
				IsBodyHtml = true
			};

			mailMessage.To.Add(to);
			smtpClient.Send(mailMessage);
		}

		public void SendVerificationEmail(string userEmail, string verificationToken)
		{
			//var request = _httpContextAccessor.HttpContext?.Request;
			//var verificationLink = $"{request?.Scheme}://{request?.Host}/VerifyAccount/{verificationToken}";
			var verificationLink = $"{_emailSettings.HostingDomain}/VerifyAccount/{verificationToken}";


			string emailBody = $@"
        <h2>Bekräfta din e-postadress</h2>
        <p>Klicka på länken nedan för att bekräfta din e-post:</p>
        <a href='{verificationLink}'>Verifiera mitt konto</a>";

			SendEmail(userEmail, "Bekräfta din e-postadress", emailBody);
		}

		public void SendPasswordResetEmail(string userEmail, string resetToken)
		{
			//var request = _httpContextAccessor.HttpContext?.Request;
			//var resetLink = $"{request?.Scheme}://{request?.Host}/ResetPassword/{resetToken}";
			var resetLink = $"{_emailSettings.HostingDomain}/ResetPassword/{resetToken}";
			string emailBody = $@"
        <h2>Återställ ditt lösenord</h2>
        <p>Klicka på länken nedan för att återställa ditt lösenord:</p>
        <a href='{resetLink}'>Återställ lösenord</a>";

			SendEmail(userEmail, "Återställ ditt lösenord", emailBody);
		}

		public class EmailSettings
		{
			public string SmtpServer { get; set; }
			public int Port { get; set; }
			public string Username { get; set; }
			public string Password { get; set; }
			public string From { get; set; }
			public string HostingDomain { get; set; }

		}
	}

}
