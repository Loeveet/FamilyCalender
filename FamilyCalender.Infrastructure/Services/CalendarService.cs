using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FamilyCalender.Infrastructure.Services
{
	public class CalendarService(ICalendarAccessService calendarAccessService, ApplicationDbContext context) : ICalendarService
    {
        private readonly ICalendarAccessService _calendarAccessService = calendarAccessService;
		private readonly ApplicationDbContext _context = context;

		public async Task<List<Calendar>> GetAllCalendarsAsync()
        {
			return await _context.Calendars
				.Include(c => c.Events)
				.Include(c => c.Accesses)
				.Include(c => c.MemberCalendars)
				.ThenInclude(mc => mc.Member)
				.ToListAsync();
		}

		public async Task<User> GetOwnerForCalendar(int calendarId)
		{
			return await _context.Calendars
				.Include(c => c.Owner)
				.ThenInclude(c => c.NotificationSetting)
				.Where(x => x.Id == calendarId)
				.Select(x => x.Owner)
				.FirstOrDefaultAsync();
		}

		public async Task<Calendar> CreateCalendarAsync(Calendar calendar, User user)
        {
            if (string.IsNullOrWhiteSpace(calendar.Name))
            {
                throw new ArgumentException("Calendar name cannot be empty.");
            }

            calendar.OwnerId = user.Id;
            calendar.Owner = user;

            try
            {
                var newCalendar = await AddAsync(calendar);

                var access = new CalendarAccess()
                {
                    Calendar = newCalendar,
                    CalendarId = newCalendar.Id,
                    User = user,
                    UserId = user.Id,
                    IsOwner = true
                };

                await _calendarAccessService.CreateCalendarAccessAsync(access);

                return newCalendar;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Something went wrong while creating the calendar.", ex);
            }
        }

        public async Task<List<int>> GetCalendarIdsForUserAsync(int userId)
        {
			//var calendarAccesses = await _context.CalendarAccesses
			//	.Where(ca => ca.UserId == userId)
			//	.Include(ca => ca.Calendar)
			//	.ThenInclude(c => c.Events)
			//	.ToListAsync();

			//var calendars = calendarAccesses
			//	.Select(ca => ca.Calendar.Id)
			//	.Distinct()
			//	.ToList();

			var calendars = await _context.CalendarAccesses
				.Where(ca => ca.UserId == userId)
				.Include(ca => ca.Calendar)
				.ThenInclude(c => c.Events)
				.Select(ca => ca.Calendar.Id)
				.Distinct()
				.ToListAsync();


			return calendars;
		}

        public Task<List<Event>> GetEventsForCalendarAsync(int calendarId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
        {
            throw new NotImplementedException();
        }

		public async Task<Calendar> GetOneCalendarAsync(int calendarId)
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

		public async Task<Calendar> UpdateCalendarAsync(Calendar calendar)
        {
			calendar.LastEditedUtc = DateTime.UtcNow;
			_context.Calendars.Update(calendar);
			await _context.SaveChangesAsync();
			return calendar;
		}

        public async Task UpdateCalendarInviteIdAsync(Calendar calendar)
        {
			await UpdateAsync(calendar);
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

		public async Task<List<CalendarDto>> GetCalendarDtosForUserAsync(int userId)
		{
			var calendars = await (
				from c in _context.Calendars
				where c.OwnerId == userId
				select new CalendarDto
				{
					Id = c.Id,
					Name = c.Name,
					InviteId = c.InviteId
				}
			)
			.Union(
				from ca in _context.CalendarAccesses
				where ca.UserId == userId
				select new CalendarDto
				{
					Id = ca.Calendar.Id,
					Name = ca.Calendar.Name,
					InviteId = ca.Calendar.InviteId
				}
			)
			.Distinct()
			.ToListAsync();

			return calendars;
		}

		public async Task UpdateCalendarNameAsync(int calendarId, string newName)
        {
            var calendar = await _context.Calendars
				.Include(c => c.MemberCalendars)
				.Include(c => c.Accesses)
				.Include(c => c.Events)
				.FirstOrDefaultAsync(c => c.Id == calendarId);

			if (calendar == null) throw new ArgumentException("Calendar not found");

            calendar.Name = newName;
            await UpdateAsync(calendar);
        }

        public async Task DeleteCalendarAsync(int calendarId)
        {
			var calendar = await GetCalendarWithAllRelationsAsync(calendarId);

			if (calendar == null)
			{
				throw new Exception("Kalendern hittades inte.");
			}

			await DeleteAsync(calendar);
		}

		private async Task<Calendar> AddAsync(Calendar calendar)
		{
			calendar.CreatedUtc = DateTime.UtcNow;
			calendar.LastEditedUtc = DateTime.UtcNow;
			_context.Calendars.Add(calendar);
			await _context.SaveChangesAsync();
			return calendar;
		}

		private async Task<Calendar?> GetCalendarWithAllRelationsAsync(int calendarId)
		{
			return await _context.Calendars
				.Include(c => c.Events)
				.Include(c => c.Accesses)
				.Include(c => c.MemberCalendars)
							.ThenInclude(mc => mc.Member)
				.FirstOrDefaultAsync(c => c.Id == calendarId);
		}

		private async Task DeleteAsync(Calendar calendar)
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
		private async Task UpdateAsync(Calendar calendar)
		{
			calendar.LastEditedUtc = DateTime.UtcNow;
			_context.Calendars.Update(calendar);
			await _context.SaveChangesAsync();
		}
	}
}
