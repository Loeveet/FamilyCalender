using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
	internal class Member
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public int CalendarId { get; set; }
		public Calendar Calendar { get; set; }

		public ICollection<Event> Events { get; set; }
	}
}
