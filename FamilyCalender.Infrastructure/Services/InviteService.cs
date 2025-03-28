using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FamilyCalender.Infrastructure.Services
{
	public class InviteService(ApplicationDbContext database)
	{
		private readonly ApplicationDbContext _database = database;

		public async Task<Calendar> GetByInviteId(Guid inviteId)
		{
			var calendar = await _database.Calendars.FirstOrDefaultAsync(x => x.InviteId == inviteId);

			if(calendar  == null)
			{
				return null; //kalendern finns inte längre - annan info?
			}

			return await Task.Run(() => calendar);
		}


        public async void Join(Guid inviteId, int userId)
        {
            var calendar = await _database.Calendars.Include(x => x.Accesses)
	            .FirstOrDefaultAsync(x => x.InviteId == inviteId);

            if (calendar != null)
            {
                if (calendar.Accesses.All(x => x.UserId != userId)) // never add existing connection
                {
                    _database.Add(new CalendarAccess()
                    {
                        CalendarId = calendar.Id,
                        UserId = userId,
                        IsOwner = false
                    });

                    await _database.SaveChangesAsync();
                }
            }
        }
    }
}
