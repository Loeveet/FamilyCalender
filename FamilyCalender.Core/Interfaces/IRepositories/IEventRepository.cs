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
        Task<Event> AddAsync(Event e);
        Task UpdateAsync(Event e);
        Task RemoveAsync(int eventId);
    }
}
