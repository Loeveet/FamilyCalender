using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
        public ICollection<MemberCalendar> MemberCalendars { get; set; } = [];
        public ICollection<EventMemberDate> EventMemberDates { get; set; } = [];

    }
}
