using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
			await _context.SaveChangesAsync();
			return e;
		}

		public async Task<IEnumerable<Event>> GetAllByMemberAsync(int memberId)
		{
			throw new NotImplementedException();
		}

		public async Task<List<Event>> GetByCalendar(int calendarId)
		{
			return await _context.Events
				.Where(e => e.CalendarId == calendarId)
				.Include(e => e.EventMemberDates)
				.ThenInclude(me => me.Member)
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
