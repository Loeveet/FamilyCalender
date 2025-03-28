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

        public async Task<Calendar> AddAsync(Calendar calendar)
        {
            _context.Calendars.Add(calendar);
            await _context.SaveChangesAsync();
            return calendar;
        }

        public async Task<List<int>> GetAllIdsByUserAsync(string userId)
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
            return await _context.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);
        }

		public async Task<CalendarDto> GetCalendarDtoAsync(int calendarId)
		{
			return await _context.Calendars
				.Where(c => c.Id == calendarId)
				.Select(c => new CalendarDto
				{
					Id = c.Id,
					Name = c.Name
				})
				.FirstOrDefaultAsync() ?? throw new FileNotFoundException();
		}
        public async Task<List<CalendarDto>> GetCalendarDtosAsync(string userId)
        {
            var ownCalendars = await _context.Calendars
                .Where(c => c.OwnerId == userId) 
                .Select(c => new CalendarDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            var accessCalendars = await _context.CalendarAccesses
                .Where(x => x.UserId == userId)
                .Select(c => new CalendarDto()
                {
                    Id = c.CalendarId,
                    Name = c.Calendar.Name
                }).ToListAsync();

            ownCalendars.AddRange(accessCalendars);

            return ownCalendars.DistinctBy(x => x.Id).ToList();
        }

        public async Task RemoveAsync(int calendarId)
        {
            throw new NotImplementedException();
        }

        public async Task<Calendar> UpdateAsync(Calendar calendar)
        {
            var existingCalendar = await _context.Calendars
                .Include(c => c.MemberCalendars)
                .FirstOrDefaultAsync(c => c.Id == calendar.Id);

            if (existingCalendar != null)
            {
                _context.Calendars.Update(calendar);
                await _context.SaveChangesAsync();
                return calendar;
            }
            return null;
        }
    }
}
