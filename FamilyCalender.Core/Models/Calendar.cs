using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
	public class Calendar
	{
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }

		public int? OwnerId { get; set; }
		public User? Owner { get; set; }

		public ICollection<Member>? Members { get; set; }
	}
}
