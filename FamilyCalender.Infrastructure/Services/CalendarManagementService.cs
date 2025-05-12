using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.Dto;
using FamilyCalender.Core.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyCalender.Core.Models;

namespace FamilyCalender.Infrastructure.Services
{
    public class CalendarManagementService(
			ICalendarService calendarService,
			IEventService eventService,
			IMemberService memberService,
			ICalendarAccessService calendarAccessService)
	{
		private readonly ICalendarService _calendarService = calendarService;
		private readonly IEventService _eventService = eventService;
		private readonly IMemberService _memberService = memberService;
		private readonly ICalendarAccessService _calendarAccessService = calendarAccessService;

		public async Task<List<int>> GetCalendarIdsForUserAsync(int userId)
		{
			return await _calendarService.GetCalendarIdsForUserAsync(userId);
		}
        public async Task<List<CalendarDto>> GetCalendarDtosForUserAsync(int userId)
        {
            return await _calendarService.GetCalendarDtosForUserAsync(userId);
        }

        public async Task<List<Event>> GetEventsForCalendarAsync(int calendarId, int year, int month)
		{
			return await _eventService.GetEventForCalendarAsync(calendarId, year, month);
		}
		public async Task<Core.Models.Entities.Calendar> GetCalendarByCalendarIdAsync(int calendarId)
		{
			return await _calendarService.GetOneCalendarAsync(calendarId);
		}
		public async Task<CalendarDto> GetCalendarDtoByIdAsync(int calendarId)
		{
			return await _calendarService.GetCalendarDtoAsync(calendarId);
		}
		public async Task<List<Member>> GetMembersForCalendarAsync(int calendarId)
		{
			return await _memberService.GetMembersForCalendarAsync(calendarId);
		}

		public async Task CreateEventAsync(NewCalendarEventSaveModel model)
		{
			await _eventService.CreateEventAsync(model);
		}

        public async Task<Core.Models.Entities.Calendar?> GetCalendarWithDetailsAsync(int calendarId)
        {
            return await _calendarService.GetOneCalendarAsync(calendarId);
        }

        public async Task UpdateCalendarNameAsync(int calendarId, string newName)
        {
            await _calendarService.UpdateCalendarNameAsync(calendarId, newName);
        }

		public async Task UpdateCalenderInviteIdAsync(Core.Models.Entities.Calendar cal)
		{
			await _calendarService.UpdateCalendarInviteIdAsync(cal);
		}

        public async Task DeleteCalendarAsync(int calendarId)
        {
            await _calendarService.DeleteCalendarAsync(calendarId);
        }
		public async Task RenameMemberAsync(int memberId, string newName)
		{
			await _memberService.UpdateMemberNameAsync(memberId, newName);
		}

		public async Task DeleteMemberAsync(int memberId)
		{
			await _memberService.DeleteMemberAsync(memberId);
		}

		public async Task<Member> AddMemberAsync(string name, int calendarId, User user)
		{
			return await _memberService.CreateMemberAndAddToCalendarAsync(name, calendarId, user);
		}
		public async Task RemoveUserFromCalendarAsync(int currentUserId, int calendarId)
		{
			await _calendarAccessService.RemoveUserFromCalendarAccessAsync(currentUserId, calendarId);
		}

		public async Task<bool> IsCalendarOwnerAsync(int calendarId, int userId)
		{
			var calendar = await _calendarService.GetOneCalendarAsync(calendarId);
			return calendar.OwnerId == userId;
		}

		public async Task<List<User>> GetPushSubscribers(int calendarId, int exceptUserId, SubscriberType type)
		{
			switch (type)
			{
				case SubscriberType.NewCalendarEvent:
					return await _eventService.GetPushSubscribersNewEvent(calendarId, exceptUserId);
				case SubscriberType.UpdateCalendarEvent:
					return await _eventService.GetPushSubscribersUpdateEvent(calendarId, exceptUserId);
				case SubscriberType.DeleteCalendarEvent:
					return await _eventService.GetPushSubscribersDeleteEvent(calendarId, exceptUserId);
			

				default:
					return new List<User>();
			}
		}

		public async Task<User> GetOwnerPushSettings(int calendarId)
		{
			return await _calendarService.GetOwnerForCalendar(calendarId);
		}
	}

	public enum SubscriberType
	{
		NewCalendarEvent,
		UpdateCalendarEvent,
		DeleteCalendarEvent,
	}
}
