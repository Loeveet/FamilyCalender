using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyCalender.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FamilyCalender.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using FamilyCalender.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace FamilyCalender.Web.Pages
{
	public class IndexModel(UserManager<User> userManager,
		ICalendarService calendarService,
		IEventService eventService,
		IMemberService memberService) : PageModel
	{
		private readonly UserManager<User> _userManager = userManager;
		private readonly ICalendarService _calendarService = calendarService;
		private readonly IEventService _eventService = eventService;
		private readonly IMemberService _memberService = memberService;


		public List<Core.Models.Calendar> Calendars { get; set; } = [];
		public Core.Models.Calendar SelectedCalendar { get; set; } = new Core.Models.Calendar();
		public List<Event> Events { get; set; } = [];
		public List<Member> Members { get; set; } = [];
		public List<DateTime> DaysInMonth { get; private set; } = [];

		public CultureInfo CultureInfo = new("sv-SE");

		[BindProperty]
		public DateTime? SelectedDate { get; set; }

		[BindProperty]
		public int CurrentYear { get; set; } = DateTime.Now.Year;

		[BindProperty]
		public int CurrentMonth { get; set; } = DateTime.Now.Month;

		[BindProperty]
		public string EventTitle { get; set; } = string.Empty;

		[BindProperty]
		public List<int> SelectedMemberIds { get; set; } = [];

		[BindProperty]
		public int SelectedCalendarId { get; set; }
		[BindProperty]
		public DateTime? StartDate { get; set; }

		[BindProperty]
		public DateTime? EndDate { get; set; }
		[BindProperty]
		public List<string>? SelectedDays { get; set; }

		public async Task<IActionResult> OnGetAsync(int? year, int? month, int? calendarId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return RedirectToPage("/Account/Login");
			}

			SetCurrentYearAndMonth(year, month);

			DaysInMonth = GenerateMonthDays(CurrentYear, CurrentMonth);

			Calendars = await _calendarService.GetCalendarsForUserAsync(user.Id);

			if (Calendars != null || Calendars.Count > 0)
			{
				await LoadSelectedCalendarData(calendarId);
				Members = await _memberService.GetMembersForCalendarAsync(SelectedCalendar.Id);
			}

			return Page();
		}
		public async Task<IActionResult> OnPostAddEventAsync()
		{

			if (!ModelState.IsValid || !IsValidInput(EventTitle, SelectedDate, SelectedMemberIds, SelectedCalendarId))
			{
				ModelState.AddModelError(string.Empty, "Titel, datum, medlem och kalender är obligatoriskt.");
				return Page();
			}

			var eventMemberDates = ValidateAndGenerateEventMemberDates(StartDate, EndDate, SelectedDays);
			if (eventMemberDates != null)
			{
				await CreateAndSaveEventAsync(EventTitle, eventMemberDates, SelectedCalendarId, SelectedMemberIds!);
			}

			else if (SelectedDate.HasValue)
			{
				eventMemberDates = new List<EventMemberDate>
		{
			new EventMemberDate { Date = SelectedDate.Value }
		};

				await CreateAndSaveEventAsync(EventTitle, eventMemberDates, SelectedCalendarId, SelectedMemberIds);
			}


			else
			{
				ModelState.AddModelError(string.Empty, "Ange ett giltigt datum eller intervall.");
				return Page();
			}

			return RedirectToPage("./Index", new
			{
				year = CurrentYear,
				month = CurrentMonth,
				calendarId = SelectedCalendarId
			});
		}
		private static List<EventMemberDate> GenerateEventMemberDatesInRangeWithWeekdays(DateTime start, DateTime end, List<string> selectedDays)
		{
			var dates = new List<EventMemberDate>();

			for (var date = start; date <= end; date = date.AddDays(1))
			{
				var culture = new CultureInfo("sv-SE");

				var dayOfWeek = culture.DateTimeFormat.GetDayName(date.DayOfWeek);

				if (selectedDays.Any(day => day.Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase)))
				{
					dates.Add(new EventMemberDate
					{
						Date = date
					});
				}
			}

			return dates;
		}

		private List<EventMemberDate>? ValidateAndGenerateEventMemberDates(DateTime? startDate, DateTime? endDate, List<string>? selectedDays)
		{
			if (!IsValidIntervalInputs(startDate, endDate, selectedDays))
				return null;

			return GenerateEventMemberDatesInRangeWithWeekdays(startDate!.Value, endDate!.Value, selectedDays!);
		}

		private async Task LoadSelectedCalendarData(int? calendarId)
		{
			var calendar = Calendars.FirstOrDefault(c => c.Id == calendarId);
			SelectedCalendar = calendar ?? Calendars.FirstOrDefault() ?? new Core.Models.Calendar();

			Events = await _eventService.GetEventForCalendarAsync(SelectedCalendar.Id);
			Members = await _memberService.GetMembersForCalendarAsync(SelectedCalendar.Id);
		}
		private static List<DateTime> GenerateMonthDays(int year, int month)
		{
			var daysCount = DateTime.DaysInMonth(year, month);
			var days = new List<DateTime>();

			for (var day = 1; day <= daysCount; day++)
			{
				days.Add(new DateTime(year, month, day));
			}

			return days;
		}
		private void SetCurrentYearAndMonth(int? year, int? month)
		{
			CurrentYear = year ?? CurrentYear;
			CurrentMonth = month ?? CurrentMonth;
		}
		private bool IsValidInput(string eventTitle, DateTime? selectedDate, List<int> memberIds, int calendarId)
		{
			return !string.IsNullOrEmpty(eventTitle) &&
				   selectedDate.HasValue &&
				   selectedDate.Value.Date >= DateTime.Now.Date &&
				   memberIds != null &&
				   memberIds.Count != 0 &&
				   calendarId > 0;
		}

		private bool IsValidIntervalInputs(DateTime? startDate, DateTime? endDate, List<string>? selectedDays)
		{
			return startDate.HasValue &&
				   endDate.HasValue &&
				   startDate <= endDate &&
				   selectedDays != null &&
				   selectedDays.Count != 0;
		}


		private async Task CreateAndSaveEventAsync(string eventTitle, List<EventMemberDate> eventMemberDates, int calendarId, List<int> memberIds)
		{
			await _eventService.CreateEventAsync(eventTitle, eventMemberDates, calendarId, memberIds);
		}


	}
}
