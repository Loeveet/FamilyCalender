using Newtonsoft.Json;
using Serilog;
using WebPush;

namespace FamilyCalender.Web.Code
{
	public class PushNotificationService
	{
		public void SendPush(PushData data, string userEmail, string endpoint, string p256dh, string auth)
		{
			var vapidDetails = new VapidDetails($"mailto:{userEmail}", GlobalSettings.VapidPublicKey, GlobalSettings.VapidPrivateKey);
			var webPushClient = new WebPushClient();

			var subscription = new PushSubscription(endpoint, p256dh, auth);

			try
			{
				 webPushClient.SendNotification(subscription, JsonConvert.SerializeObject(data), vapidDetails);
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
	}
}
