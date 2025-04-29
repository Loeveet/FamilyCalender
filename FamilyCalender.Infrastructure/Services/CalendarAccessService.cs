using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;

namespace FamilyCalender.Infrastructure.Services
{
	public class CalendarAccessService(ApplicationDbContext context) : ICalendarAccessService
    {
		private readonly ApplicationDbContext _context = context;

		public async Task CreateCalendarAccessAsync(CalendarAccess access)
        {
			_context.CalendarAccesses.Add(access);
			await _context.SaveChangesAsync();
		}
        public async Task RemoveUserFromCalendarAccessAsync(int userId, int calendarId)
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
