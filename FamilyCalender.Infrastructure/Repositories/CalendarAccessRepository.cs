using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class CalendarAccessRepository : ICalendarAccessRepository
    {
        private readonly ApplicationDbContext _context;

        public CalendarAccessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CalendarAccess access)
        {
            _context.CalendarAccesses.Add(access);
            await _context.SaveChangesAsync();
        }
        public async Task RemoveAsync(int userId, int calendarId)
        {
			var calendarAccess = _context.CalendarAccesses
				.Where(ca => ca.UserId == userId && ca.CalendarId == calendarId)
				.FirstOrDefault();

			if (calendarAccess != null)
			{
				_context.CalendarAccesses.Remove(calendarAccess);
				await _context.SaveChangesAsync();
			}
			else
			{
				throw new InvalidOperationException("Användaren är inte kopplad till denna kalender.");
			}
		}
    }
}
