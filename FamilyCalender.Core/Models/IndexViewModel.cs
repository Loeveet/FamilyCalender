using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
	public class IndexViewModel
	{
		public List<Core.Models.Calendar> Calendars { get; set; } = [];
		public Core.Models.Calendar SelectedCalendar { get; set; } = new Core.Models.Calendar();
		public List<Event> Events { get; set; } = [];
		public List<Member> Members { get; set; } = [];
		public List<DateTime> DaysInMonth { get; set; } = [];
		public CultureInfo CultureInfo { get; set; } = new CultureInfo("sv-SE");
		public DateTime? SelectedDate { get; set; }
		public int CurrentYear { get; set; } = DateTime.Now.Year;
		public int CurrentMonth { get; set; } = DateTime.Now.Month;
		public string EventTitle { get; set; } = string.Empty;
		public List<int> SelectedMemberIds { get; set; } = [];
		public int SelectedCalendarId { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public List<string>? SelectedDays { get; set; }
	}
}
