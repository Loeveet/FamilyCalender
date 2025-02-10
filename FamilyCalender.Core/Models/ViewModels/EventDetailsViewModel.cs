using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Core.Models.ViewModels
{
    public class EventDetailsViewModel
    {
        public Event? EventDetails { get; set; }
        public Member? Member { get; set; }
        public int MemberId { get; set; }
        public DateTime Day { get; set; }
        public ICollection<Member> Members { get; set; } = [];
        public string NewTitle { get; set; } = string.Empty;
        public ICollection<DayOfWeek> SelectedDays { get; set; } = [];
        public bool UpdateInterval { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
		public DateTime NewDate { get; set; }
        public int EventId { get; set; }
        public int CalendarId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public bool IsSingleEvent { get; set; } = false;
        public string FormattedDate { get; set; } = string.Empty;
        public string FormattedInterval { get; set; } = string.Empty;
        public ICollection<string> SwedishWeekdays { get; set; } = [];
        public ICollection<Member> ChosenMembers { get; set; } = [];
        public ICollection<string> SortedDaysInSwedish
        {
            get
            {
                var cultureInfo = new System.Globalization.CultureInfo("sv-SE");
                var weekOrder = new List<string> { "måndag", "tisdag", "onsdag", "torsdag", "fredag", "lördag", "söndag" };

                return SelectedDays
                    .Select(day => cultureInfo.DateTimeFormat.GetDayName(day).ToLower())
                    .OrderBy(day => weekOrder.IndexOf(day))
                    .ToList();
            }
        }

	}
}
