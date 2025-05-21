using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Models.Entities
{
	public class UserSettings
	{
		public int Id { get; set; }
		public bool PreferWeekView { get; set; }
		public bool DontScrollToToday { get; set; }
		public int CalendarAccessId { get; set; }
        public CalendarAccess CalendarAccess { get; set; } = null!;
	}
}
