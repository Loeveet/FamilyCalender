using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class CalendarAccessRepository : ICalendarAccessRepository
    {
        private readonly ApplicationDbContext _context;

        public CalendarAccessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CalendarAccess access)
        {
            _context.CalendarAccesses.Add(access);
            await _context.SaveChangesAsync();
        }
    }
}
