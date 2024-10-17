using FamilyCalender.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IServices
{
    public interface IEventService
    {
        Task<List<Event>> GetEventForCalendarAsync(int calendarId);
    }
}
