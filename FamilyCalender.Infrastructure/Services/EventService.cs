using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly ICalendarRepository _calendarRepository;
        private readonly EncryptionService _encryptionService;
        private readonly IEventRepository _eventRepository;

        public EventService(ICalendarRepository calendarRepository, EncryptionService encryptionService,
            IEventRepository eventRepository)
        {
            _calendarRepository = calendarRepository;
            _encryptionService = encryptionService;
            _eventRepository = eventRepository;
		}

		public async Task<Event> CreateEventAsync(string eventTitle, string text,string eventTime, EventCategoryColor categoryColor, List<EventMemberDate> eventMemberDates, int calendarId, List<int> memberIds)
		{
			var newEvent = new Event
			{
				Title = _encryptionService.AutoDetectEncryptStringToString(eventTitle, calendarId.ToString()),
				CalendarId = calendarId,
				Text = _encryptionService.AutoDetectEncryptStringToString(text, calendarId.ToString()),
				EventCategoryColor = categoryColor,
				EventTime = eventTime
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
			var evt =  await _eventRepository.GetByIdAsync(eventId);
            if (evt != null)
            {
                evt.Title = _encryptionService.AutoDetectDecryptStringToString(evt.Title, evt.CalendarId.ToString());
                evt.Text = _encryptionService.AutoDetectDecryptStringToString(evt.Text, evt.CalendarId.ToString());
            }

            return evt;
        }

		public async Task<List<Event>> GetEventForCalendarAsync(int calendarId, int year, int month)
        {
            var events = await _eventRepository.GetByCalendar(calendarId, year, month);
            foreach (var evt in events)
            {
                evt.Title = _encryptionService.AutoDetectDecryptStringToString(evt.Title, evt.CalendarId.ToString());
                evt.Text = _encryptionService.AutoDetectDecryptStringToString(evt.Text, evt.CalendarId.ToString());
            }

            return events;
        }
        public async Task UpdateEventAsync(Event e)
        {
            e.Title = _encryptionService.AutoDetectEncryptStringToString(e.Title, e.CalendarId.ToString());
            e.Text = _encryptionService.AutoDetectEncryptStringToString(e.Text, e.CalendarId.ToString());

            await _eventRepository.UpdateAsync(e);
		}
    }
}
