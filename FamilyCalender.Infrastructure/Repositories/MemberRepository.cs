using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        public Task<Member> AddAsync(Member member)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Member>> GetAllByCalendarAsync(int calendarId)
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetByIdAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(int memberId)
        {
            throw new NotImplementedException();
        }

        public Task<Member> UpdateAsync(Member member)
        {
            throw new NotImplementedException();
        }
    }
}
