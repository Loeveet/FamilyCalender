using Microsoft.AspNetCore.Identity;

namespace FamilyCalender.Core.Models.Entities
{
    public class User
    {
		public int Id { get; set; }
		public string PasswordHash { get; set; }
		public string Email { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public string? VerificationToken { get; set; }
		public DateTime? VerificationDateUtc { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public DateTime? LastLoggedInUtc { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public ICollection<CalendarAccess> CalendarAccesses { get; set; } = [];
        public ICollection<Calendar> OwnedCalendars { get; set; } = [];
        public ICollection<Member> Members { get; set; } = []; 
    }
}
