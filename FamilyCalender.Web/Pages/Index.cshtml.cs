using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyCalender.Core.Models;
using FamilyCalender.Core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FamilyCalender.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using FamilyCalender.Infrastructure.Services;

namespace FamilyCalender.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly ICalendarService _calendarService;
        private readonly IEventService _eventService;
        private readonly IMemberService _memberService;


        public List<Core.Models.Calendar>? Calendars { get; set; } = [];
        public Core.Models.Calendar? SelectedCalendar { get; set; }
        public List<Event>? Events { get; set; } = [];
        public List<Member>? Members { get; set; } = [];
        public List<DateTime> DaysInMonth { get; private set; } = [];
        public int CurrentYear { get; private set; } = DateTime.Now.Year;
        public int CurrentMonth { get; private set; } = DateTime.Now.Month;
        public string CalendarName { get; set; }

        public CultureInfo CultureInfo = new("sv-SE");

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
        public async Task OnGetAsync(int? year, int? month, int? calendarId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                RedirectToPage("/Account/Login");
                return;
            }
            if (year.HasValue) CurrentYear = year.Value;
            if (month.HasValue) CurrentMonth = month.Value;

            DaysInMonth = GenerateMonthDays(CurrentYear, CurrentMonth);

            Calendars = await _calendarService.GetCalendarsForUserAsync(user.Id);

            if (Calendars.Count == 0)
            {
                var testMembers = TestData.GetTestMembers();
                var testCalendar = TestData.GetTestCalendars(testMembers).FirstOrDefault();

                if (testCalendar != null)
                {
                    Calendars.Add(testCalendar);
                    SelectedCalendar = testCalendar;
                    Events = TestData.GetTestEvents(testMembers, testCalendar);
                    Members = testMembers;
                }
            }
            else
            {
                if (calendarId.HasValue && Calendars.Any(c => c.Id == calendarId.Value))
                {
                    SelectedCalendar = Calendars.FirstOrDefault(c => c.Id == calendarId.Value);
                }
                else
                {
                    SelectedCalendar = Calendars.FirstOrDefault();
                }

                if (SelectedCalendar != null)
                {
                    Events = await _eventService.GetEventForCalendarAsync(SelectedCalendar.Id);
                    Members = await _memberService.GetMembersForCalendarAsync(SelectedCalendar.Id);
                }
                CalendarName = SelectedCalendar.Name;


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

    }
}
