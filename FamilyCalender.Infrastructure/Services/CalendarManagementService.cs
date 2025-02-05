using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
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

		public async Task<List<Core.Models.Calendar>> GetCalendarsForUserAsync(string userId)
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

		public static List<DateTime> GenerateMonthDays(int year, int month)
		{
			var daysCount = DateTime.DaysInMonth(year, month);
			var days = new List<DateTime>();

			for (var day = 1; day <= daysCount; day++)
			{
				days.Add(new DateTime(year, month, day));
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
	}
}
