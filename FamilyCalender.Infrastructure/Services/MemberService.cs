using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace FamilyCalender.Infrastructure.Services
{
	public class MemberService(ApplicationDbContext context) : IMemberService
    {
		private readonly ApplicationDbContext _context = context;

		public async Task<Member> CreateMemberAsync(Member member, User user)
        {
            // TODO: Add more validations
            if (string.IsNullOrWhiteSpace(member.Name))
            {
                throw new ArgumentException("Member name cannot be empty.");
            }
            member.UserId = user.Id;
            member.User = user;
			_context.Members.Add(member);
			await _context.SaveChangesAsync();
			return member;
		}

        public async Task<Member> GetMemberByIdAsync(int memberId)
        {
			return await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId) ?? throw new FileNotFoundException();
		}

		public async Task<List<Member>> GetMembersByIdAsync(List<int> memberIds)
		{
			return await _context.Members
								 .Where(m => memberIds.Contains(m.Id))
								 .Include(m => m.EventMemberDates)
								 .ToListAsync();
		}

		public async Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
        {
			return await _context.Members
				.Include(m => m.MemberCalendars)
				.Where(m => m.MemberCalendars.Any(mc => mc.CalendarId == calendarId))
				.ToListAsync();
		}
		public async Task<Member> UpdateMemberNameAsync(int memberId, string newName)
		{
			var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId);

			if (string.IsNullOrWhiteSpace(newName))
				throw new ArgumentException("Name can't be empty");

			member.Name = newName;
			_context.Members.Update(member);
			await _context.SaveChangesAsync();
			return member;
		}

		public async Task DeleteMemberAsync(int memberId)
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
		public async Task<Member> CreateMemberAndAddToCalendarAsync(string name, int calendarId, User user)
		{
			if (string.IsNullOrWhiteSpace(name))
				throw new ArgumentException("Member name cannot be empty.");

			var member = new Member
			{
				Name = name,
				UserId = user.Id,
				User = user
			};
			_context.Members.Add(member);
			await _context.SaveChangesAsync();

			await AddMemberToCalendarAsync(member.Id, calendarId);

			return member;
		}
		private async Task AddMemberToCalendarAsync(int memberId, int calendarId)
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
