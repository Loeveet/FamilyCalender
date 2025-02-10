using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models.Entities
{
    public class CalendarAccess
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = new User();
        public int CalendarId { get; set; }
        public Calendar Calendar { get; set; } = new Calendar();
        public bool IsOwner { get; set; } = false;
    }
}
