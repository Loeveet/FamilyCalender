using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface IEventService
    {
        Task<List<Event>> GetEventForCalendarAsync(int calendarId, int year, int month);
        Task<Event> CreateEventAsync(NewCalendarEventSaveModel evt);
        Task<Event> GetEventByIdAsync(int eventId);
        Task UpdateEventAsync(Event e);
        Task DeleteEventAsync(int eventId);
        Task DeleteEventMemberDateAsync(int eventId, int memberId, DateTime day);
        Task DeleteAllEventMemberDatesAsync(int eventId, int memberId);
        Task<List<User>> GetPushSubscribers(int calendarId, int exceptUserId);
	}

    public class NewCalendarEventSaveModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string EventStartTime { get; set; }
        public string EventStopTime { get; set; }
        public EventCategoryColor CategoryColor { get; set; }
        public List<EventMemberDate> EventMemberDates { get; set; } = new();
        public int CalendarId { get; set; }
        public List<int> MemberIds { get; set; } = new();

    }
}
