using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyCalender.Core.Models;
using FamilyCalender.Core.Data;
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
        public int CurrentYear { get; private set; } = DateTime.Now.Year;
        public int CurrentMonth { get; private set; } = DateTime.Now.Month;

        public CultureInfo CultureInfo = new("sv-SE");
        public DateTime? SelectedDate { get; set; }
        public Member? SelectedMember { get; set; }

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

            if (Calendars == null || Calendars.Count == 0)
            {
                LoadTestCalendarData();
            }
            else
            {
                await LoadSelectedCalendarData(calendarId);
            }

            return Page();
        }
		public async Task<IActionResult> OnPostAddEventAsync()
		{
			var eventTitle = Request.Form["eventTitle"];
			var eventDatesString = Request.Form["selectedDate"];
			var memberId = int.Parse(Request.Form["memberId"]);
			var calendarId = int.Parse(Request.Form["calenderId"]);

			if (!IsValidInput(eventTitle, eventDatesString, memberId, calendarId))
			{
				ModelState.AddModelError(string.Empty, "Titel, datum, medlem och kalender �r obligatoriskt.");
				return Page();
			}

			var eventDates = ParseEventDates(eventDatesString);
			await CreateAndSaveEventAsync(eventTitle, eventDates, calendarId, memberId);
			return RedirectToPage("./Index");
		}
		private async Task LoadSelectedCalendarData(int? calendarId)
		{
			if (Calendars == null || Calendars.Count == 0)
			{
				// Hantera situationen d�r det inte finns n�gra kalendrar
				return;
			}

			var calendar = Calendars.FirstOrDefault(c => c.Id == calendarId);
			SelectedCalendar = calendar ?? Calendars.FirstOrDefault() ?? new Core.Models.Calendar();

			if (SelectedCalendar != null)
			{
				Events = await _eventService.GetEventForCalendarAsync(SelectedCalendar.Id);
				Members = await _memberService.GetMembersForCalendarAsync(SelectedCalendar.Id);
			}
		}
		private void LoadTestCalendarData()
        {
            var testMembers = TestData.GetTestMembers();
            var testCalendar = TestData.GetTestCalendars(testMembers).FirstOrDefault();

            if (testCalendar != null)
            {
                Calendars.Add(testCalendar);
                SelectedCalendar = testCalendar;
                Events = TestData.GetTestEvents(testMembers, testCalendar);
                Members = testMembers;
                CalendarName = testCalendar.Name;
            }
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
            if (year.HasValue) CurrentYear = year.Value;
            if (month.HasValue) CurrentMonth = month.Value;
        }
		private bool IsValidInput(string eventTitle, string eventDatesString, int memberId, int calendarId)
		{
			return !string.IsNullOrEmpty(eventTitle) &&
				   !string.IsNullOrEmpty(eventDatesString) &&
				   memberId > 0 &&
				   calendarId > 0;
		}
		private List<DateTime> ParseEventDates(string eventDatesString)
		{
			return eventDatesString
				.ToString()
				.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(d => DateTime.Parse(d.Trim()))
				.ToList();
		}
		private async Task CreateAndSaveEventAsync(string eventTitle, List<DateTime> eventDates, int calendarId, int memberId)
		{
			await _eventService.CreateEventAsync(eventTitle, eventDates, calendarId, memberId);
		}

	}
}
