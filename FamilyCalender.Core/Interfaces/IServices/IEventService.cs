using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface IEventService
    {
        Task<List<Event>> GetEventForCalendarAsync(int calendarId, int year, int month);
        Task<Event> CreateEventAsync(string eventTitle, string text, string eventTime, EventCategoryColor categoryColor, List<EventMemberDate> eventDates, int calenderId, List<int> memberId);
        Task<Event> GetEventByIdAsync(int eventId);
        Task UpdateEventAsync(Event e);
        Task DeleteEventAsync(int eventId);
        Task DeleteEventMemberDateAsync(int eventId, int memberId, DateTime day);
        Task DeleteAllEventMemberDatesAsync(int eventId, int memberId);


	}
}
