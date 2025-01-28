using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
				.Include(e => e.EventDates)
				.Include(e => e.MemberEvents)
				.ThenInclude(me => me.Member)
				.ToListAsync();
		}


		public async Task<Event?> GetByIdAsync(int eventId)
		{
			return await _context.Events
				.Include(e => e.EventDates)
				.Include(e => e.MemberEvents)
				.ThenInclude(me => me.Member)
				.Include(e => e.Calendar)
				.FirstOrDefaultAsync(e => e.Id == eventId);
		}

		public async Task<EventDate> GetEventDateByEventIdAndDateAsync(int eventId, DateTime date)
		{
			return await _context.EventDates
				.FirstOrDefaultAsync(ed => ed.EventId == eventId && ed.Date.Date == date.Date) ?? throw new Exception();
		}

		public async Task<MemberEvent> GetMemberEventByEventIdAndMemberIdAsync(int eventId, int memberId)
		{
			return await _context.MemberEvents
							.FirstOrDefaultAsync(me => me.EventId == eventId && me.MemberId == memberId) ?? throw new Exception();
		}

		public async Task RemoveAsync(Event e)
		{
			_context.Remove(e);
			await _context.SaveChangesAsync();
		}

		public async Task RemoveEventDateAsync(EventDate ed)
		{
			_context.EventDates.Remove(ed);
			await _context.SaveChangesAsync();

		}

		public async Task RemoveMemberEventAsync(MemberEvent me)
		{
			_context.MemberEvents.Remove(me);
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

	}
}
