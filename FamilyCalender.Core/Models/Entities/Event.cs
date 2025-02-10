using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; } = new Calendar();
        public ICollection<EventMemberDate> EventMemberDates { get; set; } = [];
    }
}
