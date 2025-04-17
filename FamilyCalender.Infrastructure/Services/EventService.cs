using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly ICalendarRepository _calendarRepository;
        private readonly IEventRepository _eventRepository;

        public EventService(ICalendarRepository calendarRepository
			, IEventRepository eventRepository)
        {
            _calendarRepository = calendarRepository;
            _eventRepository = eventRepository;
		}

		public async Task<Event> CreateEventAsync(string eventTitle, string text, List<EventMemberDate> eventMemberDates, int calendarId, List<int> memberIds)
		{
			var newEvent = new Event
			{
				Title = eventTitle,
				CalendarId = calendarId,
				Text = text
			};

			var addedEvent = await _eventRepository.AddAsync(newEvent);

			foreach (var memberId in memberIds)
			{
				foreach (var eventMemberDate in eventMemberDates)
				{
					var memberEventDate = new EventMemberDate
					{
						EventId = addedEvent.Id,
						MemberId = memberId,
						Date = eventMemberDate.Date
					};

					addedEvent.EventMemberDates.Add(memberEventDate);
				}
			}

			await _eventRepository.SaveChangesAsync();
			return addedEvent;
		}

		public async Task DeleteEventAsync(int eventId)
		{
			var eventToDelete = await _eventRepository.GetByIdAsync(eventId) ?? throw new EntryPointNotFoundException();
			await _eventRepository.RemoveAsync(eventToDelete);
		}

		public async Task DeleteEventMemberDateAsync(int eventId, int memberId, DateTime day)
		{
			var eventDateToRemove = await
				_eventRepository.GetMemberEventDateByEventIdAndMemberIdAsync(eventId, memberId, day);

			await _eventRepository.RemoveEventMemberDateAsync(eventDateToRemove);
		}

		public async Task DeleteAllEventMemberDatesAsync(int eventId, int memberId)
		{
			var eventMemberDates = await _eventRepository.GetEventMemberDatesByEventIdAndMemberIdAsync(eventId, memberId);
			if (eventMemberDates == null || !eventMemberDates.Any())
			{
				throw new EntryPointNotFoundException("No EventMemberDates found for the given event and member.");
			}
			foreach (var eventMemberDate in eventMemberDates)
			{
				await _eventRepository.RemoveEventMemberDateAsync(eventMemberDate);
			}
		}


		public async Task<Event> GetEventByIdAsync(int eventId)
		{
			return await _eventRepository.GetByIdAsync(eventId);
		}

		public async Task<List<Event>> GetEventForCalendarAsync(int calendarId, int year, int month)
        {
            return await _eventRepository.GetByCalendar(calendarId, year, month);
        }

		public async Task UpdateEventAsync(Event e)
		{
			await _eventRepository.UpdateAsync(e);
		}
    }
}
