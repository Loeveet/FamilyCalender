using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
	public class Calendar
	{
        public int Id { get; set; }
        public string? Name { get; set; }
		public string? OwnerId { get; set; }
		public User? Owner { get; set; }
		public ICollection<MemberCalendar> MemberCalendars { get; set; } = [];
        public ICollection<CalendarAccess> Accesses { get; set; } = [];
        public ICollection<Event> Events { get; set; } = [];

    }
}
