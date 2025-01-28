using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly ICalendarRepository _calendarRepository;
        private readonly IEventRepository _eventRepository;
		private readonly IMemberEventService _memberEventService;

        public EventService(ICalendarRepository calendarRepository, IEventRepository eventRepository, IMemberEventService memberEventService)
        {
            _calendarRepository = calendarRepository;
            _eventRepository = eventRepository;
			_memberEventService = memberEventService;
		}

		public async Task<Event> CreateEventAsync(string eventTitle, List<EventDate> eventDates, int calendarId, List<int> memberIds)
		{
			var newEvent = new Event
			{
				Title = eventTitle,
				EventDates = eventDates,
				CalendarId = calendarId
			};

			var addedEvent = await _eventRepository.AddAsync(newEvent);

			foreach (var memberId in memberIds)
			{
				await _memberEventService.CreateMemberEventAsync(memberId, addedEvent);
			}

			return addedEvent;
		}

		public async Task DeleteEventAsync(int eventId)
		{
			var eventToDelete = await _eventRepository.GetByIdAsync(eventId) ?? throw new EntryPointNotFoundException();
			await _eventRepository.RemoveAsync(eventToDelete);
		}

		public async Task<Event> GetEventByIdAsync(int eventId)
		{
			return await _eventRepository.GetByIdAsync(eventId);
		}

		public async Task<List<Event>> GetEventForCalendarAsync(int calendarId)
        {
            return await _eventRepository.GetByCalendar(calendarId);
        }

		public async Task UpdateEventAsync(Event e)
		{
			await _eventRepository.UpdateAsync(e);
		}
    }
}
