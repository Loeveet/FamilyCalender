using FamilyCalender.Core.Interfaces.IRepositories;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
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

        public async Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
        {
            return await _memberRepository.GetAllByCalendarAsync(calendarId);
        }
    }
}
