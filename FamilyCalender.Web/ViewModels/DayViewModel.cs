﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Web.Code;

namespace FamilyCalender.Web.ViewModels
{
    public class DayViewModel
    {
        public DateTime Date { get; set; }
        public string CapitalizedDayName { get; set; } = string.Empty;
        public bool IsCurrentDay { get; set; }
        public bool IsPastDay { get; set; }
        public int WeekOfYear { get; set; }
        public bool ShowWeekNumber { get; set; }
        public ICollection<EventMemberDate> Events { get; set; } = [];
        public PublicHolidayInfo? PublicHoliday { get; set; } = null;
    }

}
