using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Core.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyCalender.Core.Models;

namespace FamilyCalender.Infrastructure.Services
{
    public class CalendarManagementService
	{
		private readonly ICalendarService _calendarService;
		private readonly IEventService _eventService;
		private readonly IMemberService _memberService;
		private readonly ICalendarAccessService _calendarAccessService;

		public CalendarManagementService(
			ICalendarService calendarService,
			IEventService eventService,
			IMemberService memberService,
			ICalendarAccessService calendarAccessService)
		{
			_calendarService = calendarService;
			_eventService = eventService;
			_memberService = memberService;
			_calendarAccessService = calendarAccessService;
		}
		public async Task<List<int>> GetCalendarIdsForUserAsync(int userId)
		{
			return await _calendarService.GetCalendarIdsForUserAsync(userId);
		}
        public async Task<List<CalendarDto>> GetCalendarDtosForUserAsync(int userId)
        {
            return await _calendarService.GetCalendarDtosForUserAsync(userId);
        }

        public async Task<List<Event>> GetEventsForCalendarAsync(int calendarId, int year, int month)
		{
			return await _eventService.GetEventForCalendarAsync(calendarId, year, month);
		}
		public async Task<Core.Models.Entities.Calendar> GetCalendarByCalendarIdAsync(int calendarId)
		{
			return await _calendarService.GetOneCalendarAsync(calendarId);
		}
		public async Task<CalendarDto> GetCalendarDtoByIdAsync(int calendarId)
		{
			return await _calendarService.GetCalendarDtoAsync(calendarId);
		}
		public async Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
		{
			return await _memberService.GetMembersForCalendarAsync(calendarId);
		}

		public async Task CreateEventAsync(string eventTitle, string text, string eventTime, string eventStopTime, EventCategoryColor categoryColor, List<EventMemberDate> eventMemberDates, int calendarId, List<int> memberIds)
		{
			await _eventService.CreateEventAsync(eventTitle, text, eventTime, eventStopTime, categoryColor, eventMemberDates, calendarId, memberIds);
		}

		public static List<DayViewModel> GenerateMonthDays(int year, int month, CultureInfo cultureInfo, List<PublicHoliday> publicHolidays)
		{
			var daysCount = DateTime.DaysInMonth(year, month);
			var days = new List<DayViewModel>();

			for (var day = 1; day <= daysCount; day++)
			{
				var date = new DateTime(year, month, day);
				var weekOfYear = GetIso8601WeekOfYear(date);
				string dayName = date.ToString("dddd", cultureInfo);

				days.Add(new DayViewModel
				{
					Date = date,
					IsCurrentDay = date.Date == DateTime.Today,
					IsPastDay = date.Date < DateTime.Today,
					WeekOfYear = weekOfYear,
					ShowWeekNumber = date.DayOfWeek == DayOfWeek.Monday || date.Day == 1,
					CapitalizedDayName = char.ToUpper(dayName[0]) + dayName.Substring(1),
					PublicHoliday = publicHolidays.FirstOrDefault(x => x.DateTime.Year == year && x.DateTime.Month == month && x.DateTime.Day == day)
				});
			}

			return days;
		}

		public static List<EventMemberDate> GenerateEventMemberDatesInRangeWithWeekdays(DateTime start, DateTime end, List<string> selectedDays)
		{
			var dates = new List<EventMemberDate>();
			var culture = new CultureInfo("sv-SE");

			for (var date = start; date <= end; date = date.AddDays(1))
			{
				var dayOfWeek = culture.DateTimeFormat.GetDayName(date.DayOfWeek);

				if (selectedDays.Any(day => day.Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase)))
				{
					dates.Add(new EventMemberDate { Date = date });
				}
			}

			return dates;
		}

		public static int GetIso8601WeekOfYear(DateTime date)
		{
			var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);

			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				date = date.AddDays(3);
			}

			int weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
				date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday
			);

			if (weekNumber == 1 && date.Month == 12)
			{
				return 53;
			}

			return weekNumber;
		}

        public async Task<Core.Models.Entities.Calendar?> GetCalendarWithDetailsAsync(int calendarId)
        {
            return await _calendarService.GetOneCalendarAsync(calendarId);
        }

        public async Task UpdateCalendarNameAsync(int calendarId, string newName)
        {
            await _calendarService.UpdateCalendarNameAsync(calendarId, newName);
        }

		public async Task UpdateCalenderInviteIdAsync(Core.Models.Entities.Calendar cal)
		{
			await _calendarService.UpdateCalendarInviteIdAsync(cal);
		}

        public async Task DeleteCalendarAsync(int calendarId)
        {
            await _calendarService.DeleteCalendarAsync(calendarId);
        }
		public async Task RenameMemberAsync(int memberId, string newName)
		{
			await _memberService.UpdateMemberNameAsync(memberId, newName);
		}

		public async Task DeleteMemberAsync(int memberId)
		{
			await _memberService.DeleteMemberAsync(memberId);
		}

		public async Task<Member> AddMemberAsync(string name, int calendarId, User user)
		{
			return await _memberService.CreateMemberAndAddToCalendarAsync(name, calendarId, user);
		}
		public async Task RemoveUserFromCalendarAsync(int currentUserId, int calendarId)
		{
			await _calendarAccessService.RemoveUserFromCalendarAccessAsync(currentUserId, calendarId);
		}

		public async Task<bool> IsCalendarOwnerAsync(int calendarId, int userId)
		{
			var calendar = await _calendarService.GetOneCalendarAsync(calendarId);
			return calendar.OwnerId == userId;
		}

		public async Task<List<User>> GetPushSubscribers(int calendarId, int exceptUserId)
		{
			return await _eventService.GetPushSubscribers(calendarId, exceptUserId);
		}
	}
}
