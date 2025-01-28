using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Member>> GetAllByCalendarAsync(int calendarId)
        {
            return await _context.Members
                .Include(m => m.MemberCalendars)
                .Where(m => m.MemberCalendars.Any(mc => mc.CalendarId == calendarId))
                .ToListAsync();
        }

        public async Task<Member> GetByIdAsync(int memberId)
        {
            return await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId) ?? throw new FileNotFoundException();
        }

		public async Task<List<Member>> GetManyByIdAsync(List<int> memberIds)
		{
			return await _context.Members
								 .Where(m => memberIds.Contains(m.Id))
								 .ToListAsync();
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
