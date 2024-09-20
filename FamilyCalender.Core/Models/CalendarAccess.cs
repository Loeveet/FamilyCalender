using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
    public class CalendarAccess
	{
        public Guid Id { get; set; } = Guid.NewGuid();
        public int? UserId { get; set; }
		public User? User { get; set; }

		public int? CalendarId { get; set; }
		public Calendar? Calendar { get; set; }

		public bool IsOwner { get; set; } = false;
	}
}
