using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface IMemberService
    {
        Task<Member> CreateMemberAsync(Member member, User user);
        Task<List<Member>> GetMembersForCalendarAsync(int calendarId);
        Task<Member> GetMemberByIdAsync(int memberId);
		Task<List<Member>> GetMembersByIdAsync(List<int> memberIds);
		Task<Member> UpdateMemberNameAsync(int memberId, string newName);
		Task DeleteMemberAsync(int memberId);
		Task<Member> CreateMemberAndAddToCalendarAsync(string name, int calendarId, User user);


	}
}
