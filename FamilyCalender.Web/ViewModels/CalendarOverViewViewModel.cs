using System.Globalization;
using System.Numerics;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using Calendar = FamilyCalender.Core.Models.Entities.Calendar;

namespace FamilyCalender.Web.ViewModels
{
    public class CalendarOverViewViewModel
    {
        public ICollection<Calendar> Calendars { get; set; } = [];
        public ICollection<CalendarDto> CalendarDtos { get; set; } = [];
        public Calendar SelectedCalendar { get; set; } = new Core.Models.Entities.Calendar();
        public ICollection<Event> Events { get; set; } = [];
        public ICollection<Member> Members { get; set; } = new List<Core.Models.Entities.Member>();
        public ICollection<DayViewModel> DaysInMonth { get; set; } = [];
        public CultureInfo CultureInfo { get; set; } = new CultureInfo("sv-SE");
        public DateTime? SelectedDate { get; set; }
        public int CurrentYear { get; set; } = DateTime.Now.Year;
        public int CurrentMonth { get; set; } = DateTime.Now.Month;
        public string EventTitle { get; set; } = string.Empty;
        public ICollection<int> SelectedMemberIds { get; set; } = [];
        public int SelectedCalendarId { get; set; }
		public string SelectedCalendarName { get; set; } = string.Empty;
		public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ICollection<string> SelectedDays { get; set; } = [];
		public RepeatType RepetitionType { get; set; }


		public string CapitalizedMonthName
		{
			get
			{
				var currentMonth = new DateTime(CurrentYear, CurrentMonth, 1);
				var monthName = currentMonth.ToString("MMMM", new System.Globalization.CultureInfo("sv-SE"));
				return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monthName);
			}
		}

        public string GetCategoryColor(Event evt)
        {
            if (evt is null)
            {
                return "";
            }

            switch (evt.EventCategoryColor)
            {
                case EventCategoryColor.None:
                    return "";
                case EventCategoryColor.Blue:
                    return "event-color-category-blue";
                case EventCategoryColor.Green:
                    return "event-color-category-green";
                case EventCategoryColor.Yellow:
                    return "event-color-category-yellow";
                case EventCategoryColor.Pink:
                    return "event-color-category-pink";
                case EventCategoryColor.Red:
                    return "event-color-category-red";
                case EventCategoryColor.Purple:
                    return "event-color-category-purple";

                default:
                    return "";
            }
        }

		public string ShareLink { get; set; }
        public string EventText { get; set; }
        public EventCategoryColor SelectedCategoryColor { get; set; }
        public string EventTime { get; set; }
        public string EventStopTime { get; set; }
    }
}
