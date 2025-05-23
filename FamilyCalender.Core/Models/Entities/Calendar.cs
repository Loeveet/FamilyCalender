﻿namespace FamilyCalender.Core.Models.Entities
{
    public class Calendar
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public User? Owner { get; set; }
        public DateTime? LastEditedUtc { get; set; }
        public DateTime? CreatedUtc { get; set; }
        public ICollection<MemberCalendar> MemberCalendars { get; set; } = [];
        public ICollection<CalendarAccess> Accesses { get; set; } = [];
        public ICollection<Event> Events { get; set; } = [];
        public Guid InviteId { get; set; } = Guid.NewGuid();

    }
}
