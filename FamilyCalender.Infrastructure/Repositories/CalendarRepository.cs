using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class CalendarRepository : ICalendarRepository
    {
        private readonly ApplicationDbContext _context;

        public CalendarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Calendar> AddAsync(Calendar calendar)
        {
            //calendar.CreatedAt = DateTime.UtcNow;
            _context.Calendars.Add(calendar);
            await _context.SaveChangesAsync();
            return calendar;
        }

        public async Task<IEnumerable<Calendar>> GetAllByUserAsync(string userId)
        {
            return await _context.Calendars.Where(u => u.OwnerId == userId).ToListAsync();
        }

        public async Task<Calendar?> GetByIdAsync(int calendarId)
        {
            return await _context.Calendars.FirstOrDefaultAsync(c => c.Id == calendarId);
        }

        public async Task RemoveAsync(int calendarId)
        {
            throw new NotImplementedException();
        }

        public async Task<Calendar> UpdateAsync(Calendar calendar)
        {
            throw new NotImplementedException();
        }
    }
}
