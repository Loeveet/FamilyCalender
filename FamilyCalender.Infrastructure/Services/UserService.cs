using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FamilyCalender.Infrastructure.Services
{
	public class UserService(ApplicationDbContext context) : IUserService
	{
		private readonly ApplicationDbContext _context = context;

		public async Task<List<User>> GetAllUsersAsync()
		{
			return await _context.Users
				.Include(u => u.OwnedCalendars)
				.Include(u => u.CalendarAccesses)
				.ToListAsync();
		}
		public async Task CreateNotificationAsync(UserNotificationSetting uns)
		{
			ArgumentNullException.ThrowIfNull(uns);

			if (uns.UserId < 0)
			{
				throw new InvalidDataException("Missing UserId when saving NotificationSetting");
			}

			try
			{
				var now = DateTime.UtcNow;
				uns.LastUpdatedUtc = now;
				uns.CreatedUtc = now;
				_context.UserNotificationSettings.Add(uns);

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

		public async Task UpdateNotificationAsync(UserNotificationSetting uns)
		{
			ArgumentNullException.ThrowIfNull(uns);

			try
			{
				var dbUns = await GetNotificationSetting(uns.UserId);

				dbUns.AllowOnDeleteCalendarEvents = uns.AllowOnDeleteCalendarEvents;
				dbUns.AllowOnCalendarInviteAcceptEvents = uns.AllowOnCalendarInviteAcceptEvents;
				dbUns.AllowOnEditCalendarEvents = uns.AllowOnEditCalendarEvents;
				dbUns.AllowOnNewCalendarEvents = uns.AllowOnNewCalendarEvents;
				dbUns.AllowNotifications = uns.AllowNotifications;

				_context.UserNotificationSettings.Update(dbUns);
				uns.LastUpdatedUtc = DateTime.UtcNow;
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

		public async Task<UserNotificationSetting> GetNotificationSetting(int userId)
		{
			return await _context.UserNotificationSettings.FirstOrDefaultAsync(x => x.UserId == userId);
		}

		public async Task DeleteNotificationAsync(int userId)
		{
			var uns = await GetNotificationSetting(userId);
			if (uns != null)
			{
				_context.Remove(uns);
				await _context.SaveChangesAsync();
			}
		}


	}
}
