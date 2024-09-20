﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models
{
	internal class User
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; } 
		public ICollection<CalendarAccess> CalendarAccesses { get; set; }
		public ICollection<Calendar> OwnedCalendars { get; set; }

	}
}
