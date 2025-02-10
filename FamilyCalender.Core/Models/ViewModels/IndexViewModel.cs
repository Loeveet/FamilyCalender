﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyCalender.Core.Models.Entities;
using Calendar = FamilyCalender.Core.Models.Entities.Calendar;

namespace FamilyCalender.Core.Models.ViewModels
{
    public class IndexViewModel
    {
        public List<Calendar> Calendars { get; set; } = [];
        public Calendar SelectedCalendar { get; set; } = new Entities.Calendar();
        public List<Event> Events { get; set; } = [];
        public List<Member> Members { get; set; } = [];
        public List<DayViewModel> DaysInMonth { get; set; } = [];
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
		public string CapitalizedMonthName
		{
			get
			{
				var currentMonth = new DateTime(CurrentYear, CurrentMonth, 1);
				var monthName = currentMonth.ToString("MMMM", new System.Globalization.CultureInfo("sv-SE"));
				return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(monthName);
			}
		}
	}
}
