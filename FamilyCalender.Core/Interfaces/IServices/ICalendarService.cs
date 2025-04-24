using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calendar = FamilyCalender.Core.Models.Entities.Calendar;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface ICalendarService
    {
        Task<Calendar> CreateCalendarAsync(Calendar calendar, User user);
        Task<List<int>> GetCalendarIdsForUserAsync(int id);
        Task<Calendar?> UpdateCalendarAsync(Calendar calendar);
        Task<List<Event>> GetEventsForCalendarAsync(int calendarId);
        Task<List<Member>> GetMembersForCalendarAsync(int calendarId);
        Task<Calendar> GetOneCalendarAsync(int calendarId);
        Task<CalendarDto> GetCalendarDtoAsync(int calendarId);
        Task<List<CalendarDto>> GetCalendarDtosForUserAsync(int userId);
        Task UpdateCalendarNameAsync(int calendarId, string newName);
        Task DeleteCalendarAsync(int calendarId);
        Task UpdateCalendarInviteIdAsync(Calendar calendar);
        Task<List<Calendar>> GetAllCalendarsAsync();
    }
}
