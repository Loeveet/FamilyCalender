using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FamilyCalender.Core.Models;
using FamilyCalender.Core.Data;
using Microsoft.AspNetCore.Authorization;

namespace FamilyCalender.Web.Pages
{
    public class IndexModel() : PageModel
    {
        public List<Event>? Events { get; set; }
        public List<Member>? Members { get; set; }
        public List<DateTime> DaysInMonth { get; private set; }
        public int CurrentYear { get; private set; } = DateTime.Now.Year;
        public int CurrentMonth { get; private set; } = DateTime.Now.Month;

        public CultureInfo CultureInfo = new("sv-SE");

        public void OnGet(int? year, int? month)
        {
            if (year.HasValue) CurrentYear = year.Value;
            if (month.HasValue) CurrentMonth = month.Value;

            DaysInMonth = GenerateMonthDays(CurrentYear, CurrentMonth);

            var members = TestData.GetTestMembers();
            Members = members;

            var calendars = TestData.GetTestCalendars(members);
            Events = TestData.GetTestEvents(members, calendars[0]);

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
