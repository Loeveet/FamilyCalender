using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Core.Interfaces.IRepositories
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(int eventId);
        Task<List<Event>> GetByCalendar(int calendarId, int year, int month);
        Task<EventMemberDate> GetMemberEventDateByEventIdAndMemberIdAsync(int eventId, int memberId, DateTime date);
        Task<List<EventMemberDate>> GetEventMemberDatesByEventIdAndMemberIdAsync(int eventId, int memberId);
        Task<List<User>> GetPushSubscribers(int calendarId, int exceptUserId);
		Task<Event> AddAsync(Event e);
        Task UpdateAsync(Event e);
        Task RemoveAsync(Event e);
        Task RemoveEventMemberDateAsync(EventMemberDate emd);
        Task SaveChangesAsync();
	}
}
