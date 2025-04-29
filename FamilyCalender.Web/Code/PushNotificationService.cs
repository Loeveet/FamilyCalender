using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Services;
using Newtonsoft.Json;
using Serilog;
using WebPush;
using static FamilyCalender.Infrastructure.Services.EmailService;

namespace FamilyCalender.Web.Code
{
	public class PushNotificationService
	{
        private readonly CalendarManagementService _calendarManagementService;
        private readonly EmailSettings _emailSettings;
        private readonly WebPushClient _client;

        public PushNotificationService(CalendarManagementService calendarManagementService, EmailSettings emailSettings)
        {
            _calendarManagementService = calendarManagementService;
            _emailSettings = emailSettings;
            _client = new WebPushClient();
        }

        public async Task SendPush(NewCalendarEventSaveModel model, User currentUser)
        {
            var users = await _calendarManagementService.GetPushSubscribers(model.CalendarId, -1); // so we always get push during beta
            foreach (var pushUser in users)
            {
                if (pushUser.NotificationSetting != null)
                {
                    var pushData = new PushData()
                    {
                        Title = $"Nytt event '{model.Title}'",
                        Body = $"{model.EventMemberDates?.FirstOrDefault()?.Date:yyyy-MM-dd}, {model.Text}\n\nSkapad av användare {currentUser.Email}",
                        Url = $"{_emailSettings.HostingDomain}/CalendarOverview"
                    };

                    SendPush(pushData, pushUser.Email, pushUser.NotificationSetting.Endpoint, pushUser.NotificationSetting.P256dh, pushUser.NotificationSetting.Auth);
                    
                }

            }
        }		
        
        public void SendPush(PushData data, string userEmail, string endpoint, string p256dh, string auth)
		{
			var vapidDetails = new VapidDetails($"mailto:{userEmail}", GlobalSettings.VapidPublicKey, GlobalSettings.VapidPrivateKey);
			var subscription = new PushSubscription(endpoint, p256dh, auth);

			try
			{
                _client.SendNotification(subscription, JsonConvert.SerializeObject(data), vapidDetails);
			}
			catch (Exception e)
			{
				Log.Error($"Error sending push notification to User {userEmail}", e);
			}
		}
	}



	public class PushData
	{
		public string Title { get; set; }
		public string Body { get; set; }
        public string Url { get; set; }

	}
}
