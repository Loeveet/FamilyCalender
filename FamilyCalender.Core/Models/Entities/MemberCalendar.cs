using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models.Entities
{
    public class MemberCalendar
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; } = new Member();
        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; } = new Calendar();
    }
}
