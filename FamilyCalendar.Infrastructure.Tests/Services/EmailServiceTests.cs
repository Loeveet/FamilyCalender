using FamilyCalender.Infrastructure.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalendar.Infrastructure.Tests.Services
{
	[TestFixture]
	public class EmailServiceTests
	{
		[Test]
		public void SendMailTest()
		{
			var emailService = new EmailService(new EmailService.EmailSettings()
			{
				From = "no-reply@planeramedflera.se",
				Password = "fyHcapgyrveffehdu3",
				Port = 587,
				SmtpServer = "smtp.simply.com",
				Username = "no-reply@planeramedflera.se",
				HostingDomain = "https://localhost:7223"
			});

			emailService.SendVerificationEmail("robin.liliegren@outlook.com", "test");
			
		}
	}
}
