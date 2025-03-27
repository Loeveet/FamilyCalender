using Microsoft.AspNetCore.Identity;

namespace FamilyCalender.Core.Models.Entities
{
    public class User
    {
		public int Id { get; set; }
		public string Username { get; set; } = string.Empty;
		public string PasswordHash { get; set; }
		public string Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public ICollection<CalendarAccess> CalendarAccesses { get; set; } = [];
        public ICollection<Calendar> OwnedCalendars { get; set; } = [];
        public ICollection<Member> Members { get; set; } = []; 
    }
}
