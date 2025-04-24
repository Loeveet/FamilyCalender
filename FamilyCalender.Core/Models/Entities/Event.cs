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
        public EventCategoryColor EventCategoryColor { get; set; }
        public string EventTime { get; set; }
        public string EventStopTime { get; set; }
        public DateTime? LastEditedUtc { get; set; }
        public DateTime? CreatedUtc { get; set; }
    }

    public enum EventCategoryColor
    {
        None = 0,
        Blue = 10,
        Green = 20,
        Yellow = 30,
        Pink = 40,
        Red = 50,
        Purple = 60
    }
}
