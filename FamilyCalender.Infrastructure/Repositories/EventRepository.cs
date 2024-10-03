using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        public async Task<Event> AddAsync(Event e)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Event>> GetAllByMemberAsync(int memberId)
        {
            throw new NotImplementedException();
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
