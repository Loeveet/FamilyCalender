using Microsoft.AspNetCore.Identity;

namespace FamilyCalender.Core.Models
{
    public class User : IdentityUser
    {
		public ICollection<CalendarAccess> CalendarAccesses { get; set; } = [];
		public ICollection<Calendar> OwnedCalendars { get; set; } = [];
        public ICollection<Member> Members { get; set; } = [];
    }
}
