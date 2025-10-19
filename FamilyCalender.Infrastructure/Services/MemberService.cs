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
			var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId)
				?? throw new FileNotFoundException();

			return member;
		}

		public async Task<List<Member>> GetMembersByIdAsync(List<int> memberIds)
		{
			var members = await _context.Members
				.Where(m => memberIds.Contains(m.Id))
				.Include(m => m.EventMemberDates)
				.ToListAsync();

			return members;
		}

		public async Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
        {
			var members = await _context.Members
				.Where(m => m.MemberCalendars.Any(mc => mc.CalendarId == calendarId))
				.Include(m => m.MemberCalendars)
				.ToListAsync();

			return members;
		}
		public async Task<Member> UpdateMemberNameAsync(int memberId, string newName)
		{
			if (string.IsNullOrWhiteSpace(newName))
				throw new ArgumentException("Name can't be empty");

			var member = await _context.Members.FirstOrDefaultAsync(m => m.Id == memberId)
				?? throw new InvalidOperationException($"Member with ID {memberId} not found");

			member.Name = newName;
			await _context.SaveChangesAsync();
			return member;
		}

		public async Task DeleteMemberAsync(int memberId)
		{
			var member = await _context.Members
				.Include(m => m.MemberCalendars)
				.Include(m => m.EventMemberDates)
				.FirstOrDefaultAsync(m => m.Id == memberId)
				?? throw new InvalidOperationException($"Member with ID {memberId} not found");


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

			await AddMemberToCalendarAsync(member, calendarId);

			await _context.SaveChangesAsync();

			return member;
		}
		private Task AddMemberToCalendarAsync(Member member, int calendarId)
		{
			var mc = new MemberCalendar
			{
				Member = member,
				CalendarId = calendarId
			};
			_context.MemberCalendars.Add(mc);
			return Task.CompletedTask;
		}
	}
}
