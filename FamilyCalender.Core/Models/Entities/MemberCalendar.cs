using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models.Entities
{
    public class MemberCalendar // junktiontable för att kunna ha flera medlemmar på olika kalendrar och tvärtom
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member? Member { get; set; }
        public int CalendarId { get; set; }
        public Calendar? Calendar { get; set; }
    }
}
