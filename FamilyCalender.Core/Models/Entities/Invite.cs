using System.ComponentModel.DataAnnotations;

namespace FamilyCalender.Core.Models.Entities
{
    public class Invite
    {
        [Key]
        public Guid Id { get; set; }

        public int CalendarId { get; set; }
        public DateTime CreatedUtc { get; set; }
        public DateTime ExpireUtc { get; set; }
        public bool Used { get; set; }
    }
}
