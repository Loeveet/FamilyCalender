namespace FamilyCalender.Core.Models.Entities
{
    public class UserNotificationSetting
    {
		public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime? CreatedUtc { get; set; }
        public DateTime? LastUpdatedUtc { get; set; }

        public string Endpoint { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }

        public bool AllowNotifications { get; set; }

        public bool AllowOnNewCalendarEvents { get; set; }
        public bool AllowOnEditCalendarEvents { get; set; }
        public bool AllowOnDeleteCalendarEvents { get; set; }
        public bool AllowOnCalendarInviteAcceptEvents { get; set; }
    }
}
