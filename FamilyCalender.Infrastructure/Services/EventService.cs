using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly ICalendarRepository _calendarRepository;
        private readonly IEventRepository _eventRepository;

        public EventService(ICalendarRepository calendarRepository, IEventRepository eventRepository)
        {
            _calendarRepository = calendarRepository;
            _eventRepository = eventRepository;
        }
        public async Task<List<Event>> GetEventForCalendarAsync(int calendarId)
        {
            return await _eventRepository.GetByCalendar(calendarId);
        }
    }
}
