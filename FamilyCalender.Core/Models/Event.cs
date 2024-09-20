﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
    public class Event
	{
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Title { get; set; }
		public DateTime? Start { get; set; }
		public DateTime? End { get; set; }

		public int? MemberId { get; set; }
		public Member? Member { get; set; }
	}
}
