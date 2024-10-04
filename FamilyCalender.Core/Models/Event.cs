using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
    public class Event
	{
        public int Id { get; set; }
        public string? Title { get; set; }
		public DateTime? Start { get; set; }
		public DateTime? End { get; set; }
        public int CalendarId { get; set; }
        public Calendar? Calendar { get; set; }
        public ICollection<MemberCalendar> MemberCalendars { get; set; } = []; // För att kunna sätta event på flera medlemmar inom samma kalender
    }
}
