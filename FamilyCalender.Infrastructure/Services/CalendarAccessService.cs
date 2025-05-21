using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

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

		public async Task<CalendarAccess> GetCalendarAccessAsync(int userId, int calendarId)
		{
			var calendarAccess = await _context.CalendarAccesses
				.Where(ca => ca.UserId == userId && ca.CalendarId == calendarId)
				.Include(ca => ca.Settings)
				.FirstOrDefaultAsync();

			if (calendarAccess == null)
				throw new Exception("CalendarAccess not found");

			if (calendarAccess.Settings == null)
			{
				calendarAccess.Settings = new UserSettings
				{
					CalendarAccessId = calendarAccess.Id,
					PreferWeekView = false,
					DontScrollToToday = false
				};
				_context.UserSettings.Add(calendarAccess.Settings);
				await _context.SaveChangesAsync();
			}

			return calendarAccess;
		}

		public async Task RemoveUserFromCalendarAccessAsync(int userId, int calendarId)
        {
			var calendarAccess = await _context.CalendarAccesses
				.Where(ca => ca.UserId == userId && ca.CalendarId == calendarId)
				.FirstOrDefaultAsync();

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

		public async Task UpdateCalendarAccessSettingsAsync(UserSettings settings)
		{
			_context.UserSettings.Update(settings);
			await _context.SaveChangesAsync();
		}
	}
}
