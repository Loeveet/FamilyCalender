using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using FamilyCalender.Core.Models.Entities;


namespace FamilyCalender.Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=../FamilyCalender.Infrastructure/FamilyCalender.db");
            }
        }

        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<CalendarAccess> CalendarAccesses { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<MemberCalendar> MemberCalendars { get; set; }
        public DbSet<EventMemberDate> EventMemberDates { get; set; }
        public DbSet<Invite> Invites { get; set; }


    }
}

