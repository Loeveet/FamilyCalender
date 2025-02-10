using FamilyCalender.Core.Models.Entities;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface IEventService
    {
        Task<List<Event>> GetEventForCalendarAsync(int calendarId);
        Task<Event> CreateEventAsync(string eventTitle, List<EventMemberDate> eventDates, int calenderId, List<int> memberId);
        Task<Event> GetEventByIdAsync(int eventId);
        Task UpdateEventAsync(Event e);
        Task DeleteEventAsync(int eventId);
        Task DeleteEventMemberDateAsync(int eventId, int memberId, DateTime day);
        Task DeleteAllEventMemberDatesAsync(int eventId, int memberId);


	}
}
