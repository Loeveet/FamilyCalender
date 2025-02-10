using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Core.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class CalendarManagementService
	{
		private readonly ICalendarService _calendarService;
		private readonly IEventService _eventService;
		private readonly IMemberService _memberService;

		public CalendarManagementService(
			ICalendarService calendarService,
			IEventService eventService,
			IMemberService memberService)
		{
			_calendarService = calendarService;
			_eventService = eventService;
			_memberService = memberService;
		}

		public async Task<List<Core.Models.Entities.Calendar>> GetCalendarsForUserAsync(string userId)
		{
			return await _calendarService.GetCalendarsForUserAsync(userId);
		}

		public async Task<List<Event>> GetEventsForCalendarAsync(int calendarId)
		{
			return await _eventService.GetEventForCalendarAsync(calendarId);
		}

		public async Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
		{
			return await _memberService.GetMembersForCalendarAsync(calendarId);
		}

		public async Task CreateEventAsync(string eventTitle, List<EventMemberDate> eventMemberDates, int calendarId, List<int> memberIds)
		{
			await _eventService.CreateEventAsync(eventTitle, eventMemberDates, calendarId, memberIds);
		}

		public static List<DayViewModel> GenerateMonthDays(int year, int month, CultureInfo cultureInfo)
		{
			var daysCount = DateTime.DaysInMonth(year, month);
			var days = new List<DayViewModel>();

			for (var day = 1; day <= daysCount; day++)
			{
				var date = new DateTime(year, month, day);
				var weekOfYear = GetIso8601WeekOfYear(date);

				days.Add(new DayViewModel
				{
					Date = date,
					IsCurrentDay = date.Date == DateTime.Today,
					IsPastDay = date.Date < DateTime.Today,
					WeekOfYear = weekOfYear,
					ShowWeekNumber = date.DayOfWeek == DayOfWeek.Monday || date.Day == 1,
					FriendlyDayOfWeek = char.ToUpper(date.ToString("dddd", cultureInfo)[0]) + date.ToString("dddd", cultureInfo).Substring(1)
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


	}
}
