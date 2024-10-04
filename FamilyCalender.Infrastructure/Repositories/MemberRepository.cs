using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Context;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly ApplicationDbContext _context;

        public MemberRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Member> AddAsync(Member member)
        {
            _context.Members.Add(member);
            await _context.SaveChangesAsync();
            return member;

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
