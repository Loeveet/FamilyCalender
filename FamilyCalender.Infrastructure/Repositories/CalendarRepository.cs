using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
	public class CalendarRepository : ICalendarRepository
	{
		private readonly ApplicationDbContext _context;

		public CalendarRepository(ApplicationDbContext context)
		{
			_context = context;
		}
        public async Task<List<Calendar>> GetAllAsync()
        {
            return await _context.Calendars
                .Include(c => c.Events)
                .Include(c => c.Accesses)
                .Include(c => c.MemberCalendars)
                .ThenInclude(mc => mc.Member)
                .ToListAsync();
        }
        public async Task<Calendar> AddAsync(Calendar calendar)
        {
            _context.Calendars.Add(calendar);
            calendar.CreatedUtc = DateTime.UtcNow;
            calendar.LastEditedUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return calendar;
        }



        public async Task<List<int>> GetAllIdsByUserAsync(int userId)
		{
			var calendarAccesses = await _context.CalendarAccesses
				.Where(ca => ca.UserId == userId)
				.Include(ca => ca.Calendar)
				.ThenInclude(c => c.Events)
				.ToListAsync();

			var calendars = calendarAccesses
				.Select(ca => ca.Calendar.Id)
				.Distinct()
				.ToList();

			return calendars;

		}

		public async Task<Calendar> GetByIdAsync(int calendarId)
		{
			{
				var calendar = await _context.Calendars
					.Include(c => c.MemberCalendars)
						.ThenInclude(mc => mc.Member)
					.Include(c => c.Accesses)
						.ThenInclude(a => a.User)
					.Include(c => c.Events)
					.FirstOrDefaultAsync(c => c.Id == calendarId);

				return calendar ?? throw new ArgumentException($"Calendar with ID {calendarId} not found");
			}
		}

        public async Task<Calendar?> GetByIdWithDetailsAsync(int calendarId)
        {
            return await _context.Calendars
                .Include(c => c.MemberCalendars)
                .Include(c => c.Accesses)
                .Include(c => c.Events)
                .FirstOrDefaultAsync(c => c.Id == calendarId);
        }

        public async Task<CalendarDto> GetCalendarDtoAsync(int calendarId)
		{
			return await _context.Calendars
				.Where(c => c.Id == calendarId)
				.Select(c => new CalendarDto
				{
					Id = c.Id,
					Name = c.Name,
					InviteId = c.InviteId
				})
				.FirstOrDefaultAsync() ?? throw new FileNotFoundException();
		}
		public async Task<List<CalendarDto>> GetCalendarDtosAsync(int userId)
		{
			var ownCalendars = await _context.Calendars
				.Where(c => c.OwnerId == userId)
				.Select(c => new CalendarDto
				{
					Id = c.Id,
					Name = c.Name,
					InviteId = c.InviteId
				})
				.ToListAsync();

			var accessCalendars = await _context.CalendarAccesses
				.Where(x => x.UserId == userId)
				.Select(c => new CalendarDto()
				{
					Id = c.CalendarId,
					Name = c.Calendar.Name,
				}).ToListAsync();

			ownCalendars.AddRange(accessCalendars);

			return ownCalendars.DistinctBy(x => x.Id).ToList();
		}

		public async Task RemoveAsync(int calendarId)
		{
			throw new NotImplementedException();
		}
		public async Task UpdateAsync(Calendar calendar)
		{
            _context.Calendars.Update(calendar);
            calendar.LastEditedUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
		public async Task<Calendar?> GetCalendarWithAllRelationsAsync(int calendarId)
		{
			return await _context.Calendars
				.Include(c => c.Events)
				.Include(c => c.Accesses)
				.Include(c => c.MemberCalendars)
							.ThenInclude(mc => mc.Member)
				.FirstOrDefaultAsync(c => c.Id == calendarId);
		}

		public async Task DeleteAsync(Calendar calendar)
		{
			_context.Events.RemoveRange(calendar.Events);

			_context.CalendarAccesses.RemoveRange(calendar.Accesses);

			var membersToRemove = calendar.MemberCalendars
				.Select(mc => mc.Member)
				.ToList();

			_context.MemberCalendars.RemoveRange(calendar.MemberCalendars);
			
			_context.Members.RemoveRange(membersToRemove);

			_context.Calendars.Remove(calendar);

			await _context.SaveChangesAsync();
		}

        public async Task<Calendar> UpdateCreateAsync(Calendar calendar)
        {
            _context.Calendars.Update(calendar);
            calendar.LastEditedUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return calendar;
        }

    }
}
