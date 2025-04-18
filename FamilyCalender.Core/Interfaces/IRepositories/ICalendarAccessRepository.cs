﻿using FamilyCalender.Core.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyCalender.Core.Interfaces.IRepositories
{
    public interface ICalendarAccessRepository
    {
        Task AddAsync(CalendarAccess access);
        Task RemoveAsync(int userId, int calendarId);


	}
}
