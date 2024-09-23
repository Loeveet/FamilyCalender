using Microsoft.EntityFrameworkCore;
using FamilyCalender.Core.Models;


namespace FamilyCalender.Infrastructure.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=FamilyCalender.db");
            }
        }

        public DbSet<Calendar> Calendars { get; set; }
        public DbSet<CalendarAccess> CalendarAccesses { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<User> Users { get; set; }

    }
}

