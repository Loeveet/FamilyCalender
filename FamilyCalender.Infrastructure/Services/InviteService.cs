using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;

namespace FamilyCalender.Infrastructure.Services
{
	public class InviteService(ApplicationDbContext database)
	{
		private readonly ApplicationDbContext _database = database;

		private List<Invite> Invites = new List<Invite>()
		{
			new Invite()
			{
				Id = Guid.Parse("e1962eb8-7cc5-41da-9b50-77ef2448cccb"),
				CalendarId = 9,
				CreatedUtc = DateTime.UtcNow,
				ExpireUtc = DateTime.UtcNow.AddDays(7)
			}
		};

		public async Task<Calendar> GetByInviteId(Guid inviteId)
		{
			//call database instead
			var invite = Invites.FirstOrDefault(x => x.Id == inviteId);

			if(invite == null)
			{
				return null;
			}

			if(invite.ExpireUtc < DateTime.UtcNow) 
			{
				return null; //expired - annan info?
			}

			if (invite.Used)
			{
				return null; // used - annan info?
			}

			var calendar = _database.Calendars.FirstOrDefault(x => x.Id == invite.CalendarId);

			if(calendar  == null)
			{
				return null; //kalendern finns inte längre - annan info?
			}

			return await Task.Run(() => calendar);
		}

		public async Task<Guid> CreateInvite(int calendarId)
		{
			var invite = new Invite
			{
				CalendarId = calendarId,
				CreatedUtc = DateTime.UtcNow,
				ExpireUtc = DateTime.UtcNow.AddDays(7),
				Id = Guid.NewGuid() // let database do this later
			};

			//add to database
			Invites.Add(invite);
			return await Task.Run(() => invite.Id);
		}

        public async void Join(Guid InviteId, string userId)
        {
            var invite = Invites.FirstOrDefault(x => x.Id == InviteId);
            var calendar = _database.Calendars.FirstOrDefault(x => x.Id == invite.CalendarId);

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

                    invite.Used = true;

                    await _database.SaveChangesAsync();
                }
            }
        }
    }
}
