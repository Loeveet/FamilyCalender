using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FamilyCalender.Infrastructure.Services
{

    
	public class EventService(ApplicationDbContext context, EncryptionService encryptionService) : IEventService
    {
		private readonly ApplicationDbContext _context = context;
		private readonly EncryptionService _encryptionService = encryptionService;

		public async Task<Event> CreateEventAsync(NewCalendarEventSaveModel evt)
		{
			var newEvent = new Event
			{
				Title = _encryptionService.AutoDetectEncryptStringToString(evt.Title, evt.CalendarId.ToString()),
				CalendarId = evt.CalendarId,
				Text = _encryptionService.AutoDetectEncryptStringToString(evt.Text ?? "", evt.CalendarId.ToString()),
				EventCategoryColor = evt.EventCategoryColor,
				EventTime = evt.EventTime ?? "",
				EventStopTime = evt.EventStopTime ?? "",
				RepeatIntervalType = evt.RepeatIntervalType,
				CustomIntervalInWeeks = evt.CustomIntervalInWeeks
			};

			var addedEvent = await AddAsync(newEvent);

			foreach (var memberId in evt.MemberIds)
			{
				foreach (var eventMemberDate in evt.EventMemberDates)
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

			await _context.SaveChangesAsync();
			return addedEvent;
		}

		public async Task<List<User>> GetPushSubscribers(int calendarId, int exceptUserId)
		{
			var pushSubcriders = await _context.CalendarAccesses
							.Where(x => x.CalendarId == calendarId && x.UserId != exceptUserId)
							.Include(x => x.User)
							.ThenInclude(x => x.NotificationSetting)
							.Select(x => x.User)
							.ToListAsync();

			return pushSubcriders;
		}

		public async Task<List<User>> GetPushSubscribersNewEvent(int calendarId, int exceptUserId)
		{
			var subscribers = await _context.CalendarAccesses
							.Where(x => x.CalendarId == calendarId && x.UserId != exceptUserId)
							.Include(x => x.User)
							.ThenInclude(x => x.NotificationSetting)
							.Select(x => x.User)
							.ToListAsync();

			return [.. subscribers.Where(x => x.NotificationSetting?.AllowOnNewCalendarEvents == true)];
		}

		public async Task<List<User>> GetPushSubscribersUpdateEvent(int calendarId, int exceptUserId)
		{
			var subscribers = await _context.CalendarAccesses
							.Where(x => x.CalendarId == calendarId && x.UserId != exceptUserId)
							.Include(x => x.User)
							.ThenInclude(x => x.NotificationSetting)
							.Select(x => x.User)
							.ToListAsync();

			return [.. subscribers.Where(x => x.NotificationSetting?.AllowOnEditCalendarEvents == true)];
		}

		public async Task<List<User>> GetPushSubscribersDeleteEvent(int calendarId, int exceptUserId)
		{
			var subscribers = await _context.CalendarAccesses
							.Where(x => x.CalendarId == calendarId && x.UserId != exceptUserId)
							.Include(x => x.User)
							.ThenInclude(x => x.NotificationSetting)
							.Select(x => x.User)
							.ToListAsync();

			return [.. subscribers.Where(x => x.NotificationSetting?.AllowOnDeleteCalendarEvents == true)];
		}

		public async Task<List<User>> GetPushSubscribersInviteAcceptEvents(int calendarId, int exceptUserId)
		{
			var subscribers = await _context.CalendarAccesses
							.Where(x => x.CalendarId == calendarId && x.UserId != exceptUserId)
							.Include(x => x.User)
							.ThenInclude(x => x.NotificationSetting)
							.Select(x => x.User)
							.ToListAsync();

			return [.. subscribers.Where(x => x.NotificationSetting?.AllowOnCalendarInviteAcceptEvents == true)];
		}

		public async Task DeleteEventAsync(int eventId)
		{
			var eventToDelete = await _context.Events
				.Include(e => e.EventMemberDates)
				.ThenInclude(me => me.Member)
				.Include(e => e.Calendar)
				.FirstOrDefaultAsync(e => e.Id == eventId) ?? throw new FileNotFoundException();

			_context.Remove(eventToDelete);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteEventMemberDateAsync(int eventId, int memberId, DateTime date)
		{
			var eventDateToRemove = await _context.EventMemberDates
							.FirstOrDefaultAsync(emd => emd.EventId == eventId && emd.MemberId == memberId && emd.Date.Date == date.Date) ?? throw new Exception();

			_context.EventMemberDates.Remove(eventDateToRemove);
			await _context.SaveChangesAsync();

		}

		public async Task DeleteAllEventMemberDatesAsync(int eventId, int memberId)
		{
			var eventMemberDates = await _context.EventMemberDates
				.Where(emd => emd.EventId == eventId && emd.MemberId == memberId)
				.ToListAsync() ?? throw new Exception("No EventMemberDates found for the given event and member."); ;

			if (eventMemberDates == null || !eventMemberDates.Any())
			{
				throw new EntryPointNotFoundException("No EventMemberDates found for the given event and member.");
			}
			foreach (var eventMemberDate in eventMemberDates)
			{
				_context.EventMemberDates.Remove(eventMemberDate);
				await _context.SaveChangesAsync();
			}
		}


		public async Task<Event> GetEventByIdAsync(int eventId)
		{
			var evt = await _context.Events
				.Include(e => e.EventMemberDates)
				.ThenInclude(me => me.Member)
				.Include(e => e.Calendar)
				.FirstOrDefaultAsync(e => e.Id == eventId);

			if (evt != null)
            {
                evt.Title = _encryptionService.AutoDetectDecryptStringToString(evt.Title, evt.CalendarId.ToString());
                evt.Text = _encryptionService.AutoDetectDecryptStringToString(evt.Text, evt.CalendarId.ToString());
            }

            return evt;
        }

		public async Task<List<Event>> GetEventForCalendarAsync(int calendarId, int year, int month)
        {
            var events = await _context.Events
				.Where(e => e.CalendarId == calendarId &&
							e.EventMemberDates.Any(em => em.Date.Year == year && em.Date.Month == month))
				.Include(e => e.EventMemberDates.Where(em => em.Date.Year == year && em.Date.Month == month)) // Filtrera datum
				.ThenInclude(em => em.Member)
				.ToListAsync();

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

            await UpdateAsync(e);
		}

		private async Task<Event> AddAsync(Event e)
		{
			e.CreatedUtc = DateTime.UtcNow;
			e.LastEditedUtc = DateTime.UtcNow;
			await _context.Events.AddAsync(e);
			await _context.SaveChangesAsync();
			return e;
		}
		private async Task UpdateAsync(Event e)
		{
			ArgumentNullException.ThrowIfNull(e);

			try
			{
				e.LastEditedUtc = DateTime.UtcNow;
				_context.Events.Update(e);
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateException ex)
			{
				throw new InvalidOperationException("Ett fel uppstod vid uppdatering av eventet.", ex);
			}
			catch (Exception ex)
			{
				throw new Exception("Ett okänt fel inträffade.", ex);
			}
		}
	}
}
