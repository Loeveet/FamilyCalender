using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;
        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }
        public async Task<Member> CreateMemberAsync(Member member, User user)
        {
            // TODO: Add more validations
            if (string.IsNullOrWhiteSpace(member.Name))
            {
                throw new ArgumentException("Member name cannot be empty.");
            }
            member.UserId = user.Id;
            member.User = user;
            return await _memberRepository.AddAsync(member);
        }

        public async Task<Member> GetMemberByIdAsync(int memberId)
        {
            return await _memberRepository.GetByIdAsync(memberId);
        }

		public async Task<List<Member>> GetMembersByIdAsync(List<int> memberIds)
		{
			return await _memberRepository.GetManyByIdAsync(memberIds);
		}

		public async Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
        {
            return await _memberRepository.GetAllByCalendarAsync(calendarId);
        }
		public async Task<Member> UpdateMemberNameAsync(int memberId, string newName)
		{
			var member = await _memberRepository.GetByIdAsync(memberId);

			if (string.IsNullOrWhiteSpace(newName))
				throw new ArgumentException("Name can't be empty");

			member.Name = newName;
			return await _memberRepository.UpdateAsync(member);
		}

		public async Task DeleteMemberAsync(int memberId)
		{
			await _memberRepository.RemoveAsync(memberId);
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

			var created = await _memberRepository.AddAsync(member);
			await _memberRepository.AddMemberToCalendarAsync(created.Id, calendarId);

			return created;
		}
	}
}
