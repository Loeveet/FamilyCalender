namespace FamilyCalender.Core.Models.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int CalendarId { get; set; }
        public Calendar? Calendar { get; set; }
        public ICollection<EventMemberDate> EventMemberDates { get; set; } = [];
    }
}
