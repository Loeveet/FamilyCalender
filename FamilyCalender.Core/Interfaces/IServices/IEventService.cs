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
        Task<List<User>> GetPushSubscribersNewEvent(int calendarId, int exceptUserId);
		Task<List<User>> GetPushSubscribersUpdateEvent(int calendarId, int exceptUserId);
		Task<List<User>> GetPushSubscribersDeleteEvent(int calendarId, int exceptUserId);
        Task<List<User>> GetPushSubscribersInviteAcceptEvents(int calendarId, int exceptUserId);
	}
        


	public class NewCalendarEventSaveModel : Event
    {
        public List<int> MemberIds { get; set; } = new();

    }
}
