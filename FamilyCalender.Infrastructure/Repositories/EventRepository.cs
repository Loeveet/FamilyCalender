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
    public class EventRepository : IEventRepository
    {
        private readonly ApplicationDbContext _context;

        public EventRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Event> AddAsync(Event e)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Event>> GetAllByMemberAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Event>> GetByCalendar(int calendarId)
        {
            return await _context.Events.Where(e => e.CalendarId == calendarId).ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(int eventId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveAsync(int eventId)
        {
            throw new NotImplementedException();
        }

        public async Task<Event> UpdateAsync(Event e)
        {
            throw new NotImplementedException();
        }
    }
}
