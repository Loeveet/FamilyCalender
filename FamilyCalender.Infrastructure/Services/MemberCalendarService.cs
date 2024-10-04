using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Infrastructure.Services
{
    public class MemberCalendarService : IMemberCalendarService
    {
        public Task CreateMemberCalendarAsync(int memberId, int calendarId)
        {
            throw new NotImplementedException();
        }
    }
}
