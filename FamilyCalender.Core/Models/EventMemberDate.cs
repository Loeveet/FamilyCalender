using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
	public class EventMemberDate
	{
		public int Id { get; set; }

		// Relation till Event
		public int EventId { get; set; }
		public Event? Event { get; set; }

		// Relation till Member
		public int MemberId { get; set; }
		public Member? Member { get; set; }

		// Specifikt datum för eventet
		public DateTime Date { get; set; }
	}
}
