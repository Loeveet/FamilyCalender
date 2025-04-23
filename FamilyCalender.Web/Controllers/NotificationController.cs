using FamilyCalender.Web.Code;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using Serilog;
using WebPush;

namespace FamilyCalender.Web.Controllers
{
	[Authorize]
	public class NotificationController : Controller
	{
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public NotificationController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }
		public string Index()
		{
			return "Controller working";
		}

		[HttpPost]
		public async Task<IActionResult> Register([FromBody]DeviceModel data)
		{
			try
			{
				var userEmail = HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value;
                var user = await _authService.GetUserByEmailAsync(userEmail);

                if (user == null)
                {
					//return bajs
                }

                var notificationSettings = user.NotificationSetting;
                var isNewSetting = false;
                if (notificationSettings is null)
                {
                    isNewSetting = true;
					notificationSettings = new UserNotificationSetting()
                    {
                        UserId = user.Id
                    };
                }

                notificationSettings.Endpoint = data.Payload.Endpoint;
                notificationSettings.P256dh = data.Payload.Keys.P256dh;
                notificationSettings.Auth = data.Payload.Keys.Auth;
                notificationSettings.AllowNotifications = true;
                notificationSettings.AllowOnCalendarInviteAcceptEvents = true;
                notificationSettings.AllowOnDeleteCalendarEvents = true;
                notificationSettings.AllowOnEditCalendarEvents = true;
                notificationSettings.AllowOnNewCalendarEvents = true;

                try
                {
                    if (isNewSetting)
                    {
                        await _userService.CreateNotificationAsync(notificationSettings);
                    }

                    else //should never be update but just in case
                    {
                        await _userService.UpdateNotificationAsync(notificationSettings);
                    }
                    
                    new PushNotificationService().SendPush(new PushData { Title = "Välkommen till Push", Body = "Du kommer nu ta del av pushnotiser från PMF" },
                        userEmail, data.Payload.Endpoint, data.Payload.Keys.P256dh, data.Payload.Keys.Auth);

                    return new JsonResult(new { success = true});
                }
                catch (Exception e)
                {
					Log.Error("Error when saving or sending first notification", e);
                    return new JsonResult(new {success = false, message = "Sorry! Något gick fel vid registreringen av Push! Vänligen kontakta oss så kan vi flesöka närmare"});
                }
			}
			catch (System.Exception e)
			{
                Log.Error("Error when saving or sending first notification. Probelby invalid user", e);
                return new JsonResult(new { success = false, message = "Sorry! Något gick fel vid registreringen av Push! Vänligen kontakta oss så kan vi flesöka närmare" });
            }
		}

		private void GenerateVadidKeysForPush()
		{
			//VapidDetails vapidKeys = VapidHelper.GenerateVapidKeys();

			//Public key in notificationService.js and both in C# backennd
			//Never cgange these after going live
			//Console.WriteLine("Public {0}", vapidKeys.PublicKey);
			//Console.WriteLine("Private {0}", vapidKeys.PrivateKey);
		}
	}




	public class DeviceModel
	{
		public DeviceSubscription Payload { get; set; }
	}

	public class DeviceSubscription
	{
		public string Endpoint { get; set; }
		public DeviceSubscriptionKey Keys { get; set; }

		public class DeviceSubscriptionKey
		{
			public string P256dh { get; set; }
			public string Auth { get; set; }
		}
	}
}
