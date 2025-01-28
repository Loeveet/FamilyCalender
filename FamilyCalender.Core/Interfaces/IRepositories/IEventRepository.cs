using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IRepositories
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(int eventId);
        Task<IEnumerable<Event>> GetAllByMemberAsync(int memberId);
        Task<List<Event>> GetByCalendar(int calendarId);
        Task <EventDate> GetEventDateByEventIdAndDateAsync(int eventId, DateTime date);
		Task<MemberEvent> GetMemberEventByEventIdAndMemberIdAsync(int eventId, int memberId);
		Task<Event> AddAsync(Event e);
        Task UpdateAsync(Event e);
        Task RemoveAsync(Event e);
        Task RemoveEventDateAsync(EventDate ed);
        Task RemoveMemberEventAsync(MemberEvent me);
    }
}
