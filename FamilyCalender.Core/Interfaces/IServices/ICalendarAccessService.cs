using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface ICalendarAccessService
    {
        Task CreateCalendarAccessAsync(CalendarAccess access);
        Task RemoveUserFromCalendarAccessAsync(int userId, int calendarId);

	}
}
