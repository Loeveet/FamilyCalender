using Microsoft.AspNetCore.Identity;

namespace FamilyCalender.Core.Models.Entities
{
    public class User : IdentityUser
    {
        public ICollection<CalendarAccess> CalendarAccesses { get; set; } = []; //Tillgång till delade kalendrar
        public ICollection<Calendar> OwnedCalendars { get; set; } = []; //Kalendrar som user äger
        public ICollection<Member> Members { get; set; } = []; // Medlemmar, inte users
    }
}
