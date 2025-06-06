using FamilyCalender.Web.ViewModels;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Web.Code;
using FamilyCalender.Core.Interfaces.IServices;

namespace FamilyCalender.Web.Pages
{
	public class EventDetailsModel(EventManagementService eventManagementService, PushNotificationService pushNotificationService, IAuthService authService) : BasePageModel(authService)
	{
		private readonly EventManagementService _eventManagementService = eventManagementService;

		[BindProperty]
		public EventDetailsViewModel ViewModel { get; set; } = new EventDetailsViewModel();

		public async Task<IActionResult> OnGetAsync(int eventId, int memberId, DateTime day)
		{
			ViewModel.EventDetails = await _eventManagementService.GetEventDetailsAsync(eventId);

			if (ViewModel.EventDetails == null)
			{
				return NotFound();
			}
			var culture = new CultureInfo("sv-SE");

			ViewModel.Member = await _eventManagementService.GetMemberAsync(memberId);
			ViewModel.Day = day;
			ViewModel.Members = await _eventManagementService.GetMembersForCalendarAsync(ViewModel.EventDetails.CalendarId);

			ViewModel.ChosenMembers = ViewModel.EventDetails.EventMemberDates
				.Select(x => x.Member)
				.Where(m => m != null)
				.Distinct()
				.ToList()!;

			ViewModel.SelectedDays = ViewModel.EventDetails.EventMemberDates
				.Select(ed => culture.DateTimeFormat.GetDayName(ed.Date.DayOfWeek))
				.Distinct()
				.ToList();

			ViewModel.IsSingleEvent = ViewModel.EventDetails.EventMemberDates
				.Select(x => x.Date)
				.Distinct()
				.Count() == 1;

			ViewModel.IsSingleMember = ViewModel.EventDetails.EventMemberDates
				.Select(x => x.Member)
				.Distinct()
				.ToList()
				.Count() < 2;

			var orderedDates = ViewModel.EventDetails.EventMemberDates
				.Select(x => x.Date)
				.Distinct()
				.OrderBy(d => d)
				.ToList();

			ViewModel.FormattedDate = ViewModel.IsSingleEvent ?
				orderedDates.First().ToString("yyyy-MM-dd") : string.Empty;

			ViewModel.FormattedInterval = !ViewModel.IsSingleEvent ?
				$"{orderedDates.First():yyyy-MM-dd} - {orderedDates.Last():yyyy-MM-dd}" : string.Empty;


			return Page();
		}

		public async Task<IActionResult> OnPostUpdateEventAsync()
		{
			//if (!ModelState.IsValid)
			//{
			//	return Page();
			//}

			var eventToUpdate = await _eventManagementService.GetEventDetailsAsync(ViewModel.EventId);
			if (eventToUpdate == null)
			{
				return NotFound();
			}

			eventToUpdate.Title = ViewModel.NewTitle;
			eventToUpdate.Text = ViewModel?.EventDetails?.Text ?? "";
            eventToUpdate.EventTime = ViewModel?.EventDetails?.EventTime ?? "";
            eventToUpdate.EventStopTime = ViewModel?.EventDetails?.EventStopTime ?? "";
			eventToUpdate.EventCategoryColor = ViewModel?.EventDetails?.EventCategoryColor ?? EventCategoryColor.None;

			await _eventManagementService.UpdateEventAsync(eventToUpdate);

			await pushNotificationService.SendPush(eventToUpdate, false, await GetCurrentUserAsync()); 

			return RedirectToPage("./EventDetails", new { 
				eventId = eventToUpdate.Id, 
				memberId = ViewModel.MemberId, 
				day = ViewModel.Day 
			});
		}

		public async Task<IActionResult> OnPostDeleteEventAsync(List<int> selectedMemberIds, string? deleteOption)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			await _eventManagementService.DeleteEventAsync(ViewModel.EventId, ViewModel.MemberId, ViewModel.Day, selectedMemberIds, deleteOption);
			await pushNotificationService.SendPush(ViewModel.EventDetails, true, await GetCurrentUserAsync());

			return RedirectToPage("./CalendarOverview", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
		}
		public async Task<IActionResult> OnPostRouteToIndexAsync()
		{
			return RedirectToPage("./CalendarOverview", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
		}


	}
}
