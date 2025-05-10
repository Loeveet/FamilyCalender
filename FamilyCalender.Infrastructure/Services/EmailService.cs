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


            //string emailBody = $@"
            //     <h2>Bekräfta din e-postadress</h2>
            //     <p>Klicka på länken nedan för att bekräfta din e-post:</p>
            //     <a href='{verificationLink}'>Verifiera mitt konto</a>";
            string emailBody = $@"
            <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f9;
                            margin: 0;
                            padding: 0;
                        }}
                        .email-container {{
                            max-width: 600px;
                            margin: 50px auto;
                            background-color: #ffffff;
                            padding: 30px;
                            border-radius: 8px;
                            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            text-align: center;
                        }}
                        .email-header {{
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            margin-bottom: 20px;
                        }}
                        .logo {{
                            height: 32px;
                            width: 32px;
                            margin-left: 10px;

                        }}
                        .header {{
                            color: #4a90e2;
                            margin: 0;
                        }}
                        .button {{
                            display: inline-block;
                            background-color: #4a90e2;
                            color: white;
                            padding: 15px 30px;
                            text-decoration: none;
                            font-size: 16px;
                            border-radius: 4px;
                            margin-top: 20px;
                            text-align: center;
                        }}
                        .button:hover {{
                            background-color: #357ab7;
                        }}
                        .footer {{
                            text-align: center;
                            color: #888;
                            font-size: 12px;
                            margin-top: 30px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <div class='email-header'>
                            <h2 class='header'>Välkommen till PlaneraMedFlera</h2>
                            <a href='https://planeramedflera.se' target='_blank'>
                                <img src='https://planeramedflera.se/images/favicon-32x32.png' alt='PlaneraMedFlera.se' class='logo' />
                            </a>
                        </div>
                        <p>Vi är glada att du har skapat ett konto. Nu är det dags att bekräfta din e-postadress så att du kan skapa dig eller gå med i din första kalender!</p>
                        <p>Klicka på knappen nedan för att bekräfta din e-postadress och aktivera ditt konto:</p>
                        <a href='{verificationLink}' class='button'>Verifiera mitt konto</a>
                        <div class='footer'>
                            <p>Om du inte skapade detta konto, kan du ignorera detta e-postmeddelande.</p>
                        </div>
                    </div>
                </body>
            </html>";


            SendEmail(userEmail, "Bekräfta din e-postadress", emailBody);
        }

        public void SendPasswordResetEmail(string userEmail, string resetToken)
        {
            //var request = _httpContextAccessor.HttpContext?.Request;
            //var resetLink = $"{request?.Scheme}://{request?.Host}/ResetPassword/{resetToken}";
            var resetLink = $"{_emailSettings.HostingDomain}/ResetPassword/{resetToken}";
            string emailBody = $@"
            <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f9;
                            margin: 0;
                            padding: 0;
                        }}
                        .email-container {{
                            max-width: 600px;
                            margin: 50px auto;
                            background-color: #ffffff;
                            padding: 30px;
                            border-radius: 8px;
                            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                            text-align: center;
                        }}
                        .email-header {{
                            display: flex;
                            align-items: center;
                            justify-content: center;
                            margin-bottom: 20px;
                        }}
                        .logo {{
                            height: 32px;
                            width: 32px;
                            margin-left: 10px;
                        }}
                        .header {{
                            color: #4a90e2;
                            margin: 0;
                        }}
                        .button {{
                            display: inline-block;
                            background-color: #4a90e2;
                            color: white;
                            padding: 15px 30px;
                            text-decoration: none;
                            font-size: 16px;
                            border-radius: 4px;
                            margin-top: 20px;
                            text-align: center;
                        }}
                        .button:hover {{
                            background-color: #357ab7;
                        }}
                        .footer {{
                            text-align: center;
                            color: #888;
                            font-size: 12px;
                            margin-top: 30px;
                        }}
                    </style>
                </head>
                <body>
                    <div class='email-container'>
                        <div class='email-header'>
                            <h2 class='header'>Glömde du ditt lösenord?</h2>
                            <a href='https://planeramedflera.se' target='_blank'>
                                <img src='https://planeramedflera.se/images/favicon-32x32.png' alt='PlaneraMedFlera.se' class='logo' />
                            </a>
                        </div>
                        <p>Oroa dig inte, vi har allt under kontroll! Klicka på länken nedan för att återställa ditt lösenord och få tillgång till ditt konto igen:</p>
                        <a href='{resetLink}' class='button'>Återställ lösenord</a>
                        <div class='footer'>
                            <p>Om du inte begärde en lösenordsåterställning, kan du ignorera detta e-postmeddelande.</p>
                        </div>
                    </div>
                </body>
            </html>";

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
