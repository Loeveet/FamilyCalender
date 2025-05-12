using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using WebPush;
using static FamilyCalender.Infrastructure.Services.EmailService;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FamilyCalender.Web.Code
{
    public class PushNotificationService
    {
        private readonly CalendarManagementService _calendarManagementService;
        private readonly EmailSettings _emailSettings;
        private readonly EncryptionService _encryptionService;
        private readonly WebPushClient _client;

        public PushNotificationService(CalendarManagementService calendarManagementService, EmailSettings emailSettings, EncryptionService encryptionService)
        {
            _calendarManagementService = calendarManagementService;
            _emailSettings = emailSettings;
            _encryptionService = encryptionService;
            _client = new WebPushClient();
        }

        public async Task SendPush(NewCalendarEventSaveModel model, User currentUser)
        {
            if (model != null)
            {
                var users = await _calendarManagementService.GetPushSubscribers(model.CalendarId, currentUser.Id, SubscriberType.NewCalendarEvent);
                foreach (var pushUser in users)
                {
                    if (pushUser.NotificationSetting != null)
                    {
                        var pushData = new PushData()
                        {
                            Title = $"Nytt event '{model.Title}' - {model.Calendar?.Name}",
                            Body = $"{model.EventMemberDates?.FirstOrDefault()?.Date:yyyy-MM-dd}, {model.Text}\nSkapad av användare {currentUser.Email}",
                            Url = $"{_emailSettings.HostingDomain}/CalendarOverview"
                        };

                        SendPush(pushData, pushUser.Email, pushUser.NotificationSetting.Endpoint, pushUser.NotificationSetting.P256dh, pushUser.NotificationSetting.Auth);
                    }
                }
            }
        }

        public async Task SendPush(Event model, bool delete, User currentUser)
        {
            if (model != null)
            {
                var title = _encryptionService.AutoDetectDecryptStringToString(model.Title, model.CalendarId.ToString());
                var text = _encryptionService.AutoDetectDecryptStringToString(model.Text, model.CalendarId.ToString());
                var heading = delete ? "Raderat" : "Uppdaterat";
                var users = await _calendarManagementService.GetPushSubscribers(model.CalendarId, currentUser.Id, delete ? SubscriberType.DeleteCalendarEvent : SubscriberType.UpdateCalendarEvent); // so we always get push during beta
                foreach (var pushUser in users)
                {
                    if (pushUser.NotificationSetting != null)
                    {
                        var firstMemberDate = model.EventMemberDates?.FirstOrDefault();
                        if (firstMemberDate != null)
                        {
                            var eventId = model.Id; // Här får du det faktiska EventId
                            var memberId = firstMemberDate.MemberId;
                            var date = firstMemberDate.Date.ToString("yyyy-MM-dd");

                            var pushData = new PushData()
                            {
                                Title = $"{heading} event '{title}' - {model.Calendar?.Name}",
                                Body = $"{model.EventMemberDates?.FirstOrDefault()?.Date:yyyy-MM-dd}, {text}\nSkapad av användare {currentUser.Email}",
                                //Url = $"{_emailSettings.HostingDomain}/CalendarOverview"

                                Url = $"{_emailSettings.HostingDomain}/EventDetails?eventId={eventId}&memberId={memberId}&day={date}"
                            };

                            SendPush(pushData, pushUser.Email, pushUser.NotificationSetting.Endpoint, pushUser.NotificationSetting.P256dh, pushUser.NotificationSetting.Auth);

                        }
                    }
                }
            }
        }

        public void SendPush(PushData data, string userEmail, string endpoint, string p256dh, string auth)
        {
            var vapidDetails = new VapidDetails($"mailto:{userEmail}", GlobalSettings.VapidPublicKey, GlobalSettings.VapidPrivateKey);
            var subscription = new PushSubscription(endpoint, p256dh, auth);

            if (string.IsNullOrWhiteSpace(data.Url))
            {
                data.Url = $"{_emailSettings.HostingDomain}/CalendarOverview";
            }


            try
            {
                _client.SendNotification(subscription, JsonConvert.SerializeObject(data), vapidDetails);
            }
            catch (Exception e)
            {
                Log.Error($"Error sending push notification to User {userEmail}", e);
            }
        }

        public async Task SendAcceptCalendarInvitePush(int? calendarId, string calendarName, string joiningUserEmail)
        {
            if (calendarId != null)
            {
                var user = await _calendarManagementService.GetOwnerPushSettings(calendarId.Value);
                if (user != null && user.NotificationSetting != null)
                {
                    SendPush(new PushData
                    {
                        Title = "Accepterat kalenderinbjudan",
                        Body = $"{joiningUserEmail} har anslutit till din kalender {calendarName}",
                        Url = $"{_emailSettings.HostingDomain}/CalendarOverview"
                    }, user.Email, user.NotificationSetting.Endpoint, user.NotificationSetting.P256dh, user.NotificationSetting.Auth);
                }
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
