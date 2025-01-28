using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
    public class Member
	{
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public ICollection<MemberCalendar> MemberCalendars { get; set; } = []; 
        public ICollection<MemberEvent> MemberEvents { get; set; } = [];
		public ICollection<EventMemberDate> EventMemberDates { get; set; } = [];

	}
}
