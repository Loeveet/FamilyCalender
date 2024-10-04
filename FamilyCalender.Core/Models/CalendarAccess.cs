using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
    public class CalendarAccess // För att ge specifik användare tillgång till kalender och även indikera om man är ägare eller inte
	{
        public int Id { get; set; }
        public string? UserId { get; set; }
		public User? User { get; set; }
		public int CalendarId { get; set; }
		public Calendar? Calendar { get; set; }
		public bool IsOwner { get; set; } = false;
	}
}
