using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface ICalendarService
    {
        Task<Calendar> CreateCalendarAsync(Calendar calendar, User user);
        Task<List<Calendar>> GetCalendarsForUserAsync(string id);
        Task<Calendar?> UpdateCalendarAsync(Calendar calendar);
        Task<List<Event>> GetEventsForCalendarAsync(int calendarId);
        Task<List<Member>> GetMembersForCalendarAsync(int calendarId);


    }
}
