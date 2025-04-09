using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Models.Entities;
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
                                 .Include(m => m.EventMemberDates)
								 .ToListAsync();
		}

		public async Task<Member> UpdateAsync(Member member)
		{
			_context.Members.Update(member);
			await _context.SaveChangesAsync();
			return member;
		}


		/// <summary>
		/// Removes a member and all their related data (MemberCalendars, EventMemberDates)
		/// </summary>
		public async Task RemoveAsync(int memberId)
		{
			var member = await _context.Members
				.Include(m => m.MemberCalendars)
				.Include(m => m.EventMemberDates)
				.FirstOrDefaultAsync(m => m.Id == memberId);

			if (member == null) throw new FileNotFoundException();

			_context.EventMemberDates.RemoveRange(member.EventMemberDates);
			_context.MemberCalendars.RemoveRange(member.MemberCalendars);
			_context.Members.Remove(member);
			await _context.SaveChangesAsync();
		}
		public async Task AddMemberToCalendarAsync(int memberId, int calendarId)
		{
			var mc = new MemberCalendar
			{
				MemberId = memberId,
				CalendarId = calendarId
			};
			_context.MemberCalendars.Add(mc);
			await _context.SaveChangesAsync();
		}
	}
}
