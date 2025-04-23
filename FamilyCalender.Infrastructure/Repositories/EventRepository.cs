using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
	{
		private readonly ApplicationDbContext _context;

		public EventRepository(ApplicationDbContext context)
		{
			_context = context;
		}
		public async Task<Event> AddAsync(Event e)
		{
			await _context.Events.AddAsync(e);
            e.CreatedUtc = DateTime.UtcNow;
            e.LastEditedUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
			return e;
		}

		public async Task<List<Event>> GetByCalendar(int calendarId, int year, int month)
		{
			return await _context.Events
				.Where(e => e.CalendarId == calendarId &&
							e.EventMemberDates.Any(em => em.Date.Year == year && em.Date.Month == month))
				.Include(e => e.EventMemberDates.Where(em => em.Date.Year == year && em.Date.Month == month)) // Filtrera datum
				.ThenInclude(em => em.Member)
				.ToListAsync();
		}

		public async Task<List<User>> GetPushSubscribers(int calendarId, int exceptUserId)
		{
			return await _context.CalendarAccesses
				.Where(x => x.CalendarId == calendarId && x.UserId != exceptUserId)
				.Include(x => x.User)
				.ThenInclude(x => x.NotificationSetting)
				.Select(x => x.User)
				.ToListAsync();
		}


		public async Task<Event?> GetByIdAsync(int eventId)
		{
			return await _context.Events
				.Include(e => e.EventMemberDates)
				.ThenInclude(me => me.Member)
				.Include(e => e.Calendar)
				.FirstOrDefaultAsync(e => e.Id == eventId);
		}
		public async Task<EventMemberDate> GetMemberEventDateByEventIdAndMemberIdAsync(int eventId, int memberId, DateTime date)
		{
			return await _context.EventMemberDates
							.FirstOrDefaultAsync(emd => emd.EventId == eventId && emd.MemberId == memberId && emd.Date.Date == date.Date) ?? throw new Exception();
		}
		public async Task<List<EventMemberDate>> GetEventMemberDatesByEventIdAndMemberIdAsync(int eventId, int memberId)
		{
			var eventMemberDates = await _context.EventMemberDates
				.Where(emd => emd.EventId == eventId && emd.MemberId == memberId)
				.ToListAsync() ?? throw new Exception("No EventMemberDates found for the given event and member."); ;


			return eventMemberDates;
		}

		public async Task RemoveAsync(Event e)
		{
			_context.Remove(e);
			await _context.SaveChangesAsync();
		}

		public async Task RemoveEventMemberDateAsync(EventMemberDate emd)
		{
			_context.EventMemberDates.Remove(emd);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(Event e)
		{
			ArgumentNullException.ThrowIfNull(e);

			try
			{
				_context.Events.Update(e);
                e.LastEditedUtc = DateTime.UtcNow;
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

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
