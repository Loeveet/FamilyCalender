using Microsoft.AspNetCore.Identity;

namespace FamilyCalender.Core.Models.Entities
{
    public class User : IdentityUser
    {
        public ICollection<CalendarAccess> CalendarAccesses { get; set; } = [];
        public ICollection<Calendar> OwnedCalendars { get; set; } = [];
        public ICollection<Member> Members { get; set; } = []; 
    }
}
