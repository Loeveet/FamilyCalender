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
        public ICollection<string> SelectedDays { get; set; } = [];
        public bool UpdateInterval { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; } = DateTime.Now;
		public DateTime NewDate { get; set; }
        public int EventId { get; set; }
        public int CalendarId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public bool IsSingleEvent { get; set; } = false;
		public bool IsSingleMember { get; set; } = false;
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
					.Select(day => day.ToLower()) 
					.OrderBy(day => weekOrder.IndexOf(day))
					.ToList();
			}
        }

        public string GetCategoryColorName()
        {
	        if (EventDetails is null)
	        {
		        return "Ingen";
	        }

	        switch (EventDetails.EventCategoryColor)
	        {
                case EventCategoryColor.None:
	                return "Ingen";
                case EventCategoryColor.Blue:
	                return "Blå";
                case EventCategoryColor.Green:
	                return "Grön";
                case EventCategoryColor.Yellow:
	                return "Gul";
                case EventCategoryColor.Pink:
	                return "Rosa";
                case EventCategoryColor.Red:
	                return "Röd";
                case EventCategoryColor.Purple:
	                return "Lila";

                default:
	                return "Ingen";
			}
        }

        public string GetCategoryColorDiamond()
        {
	        if (EventDetails is null)
	        {
		        return "";
	        }

	        switch (EventDetails.EventCategoryColor)
	        {
		        case EventCategoryColor.None:
			        return "";
		        case EventCategoryColor.Blue:
			        return "\ud83d\udd35";
		        case EventCategoryColor.Green:
			        return "\ud83d\udfe2";
		        case EventCategoryColor.Yellow:
			        return "\ud83d\udfe1";
		        case EventCategoryColor.Pink:
			        return "\ud83c\udf38";
		        case EventCategoryColor.Red:
			        return "\ud83d\udd34";
		        case EventCategoryColor.Purple:
			        return "\ud83d\udfe3";

		        default:
			        return "";
	        }
        }
	}
}
