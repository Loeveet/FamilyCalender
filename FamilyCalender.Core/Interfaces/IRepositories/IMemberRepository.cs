using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IRepositories
{
    public interface IMemberRepository
    {
        Task<Member> GetByIdAsync(int memberId);
        Task<List<Member>> GetAllByCalendarAsync(int calendarId);
        Task<Member> AddAsync(Member member);
        Task<Member> UpdateAsync(Member member);
        Task RemoveAsync(int memberId);
    }
}
