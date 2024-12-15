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
	public class IndexModel : PageModel
	{
		private readonly UserManager<User> _userManager;
		private readonly ICalendarService _calendarService;
		private readonly IEventService _eventService;
		private readonly IMemberService _memberService;


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


		public IndexModel(UserManager<User> userManager,
			ICalendarService calendarService,
			IEventService eventService,
			IMemberService memberService)
		{
			_calendarService = calendarService;
			_eventService = eventService;
			_memberService = memberService;
			_userManager = userManager;
		}
		public async Task<IActionResult> OnGetAsync(int? year, int? month, int? calendarId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				RedirectToPage("/Account/Login");
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

			var eventDates = SelectedDate.HasValue
							? new List<DateTime> { SelectedDate.Value }
							: []; 
			
			await CreateAndSaveEventAsync(EventTitle, eventDates, SelectedCalendarId, SelectedMemberIds);

			return RedirectToPage("./Index", new
			{
				year = CurrentYear,
				month = CurrentMonth,
				calendarId = SelectedCalendarId
			});
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
				   memberIds != null &&
				   memberIds.Any() &&
				   calendarId > 0;
		}

		private async Task CreateAndSaveEventAsync(string eventTitle, List<DateTime> eventDates, int calendarId, List<int> memberIds)
		{
			await _eventService.CreateEventAsync(eventTitle, eventDates, calendarId, memberIds);
		}


	}
}
