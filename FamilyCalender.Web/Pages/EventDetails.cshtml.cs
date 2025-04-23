using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models.ViewModels;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Packaging;
using System.Globalization;
using FamilyCalender.Core.Models.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FamilyCalender.Web.Pages
{
	public class EventDetailsModel(EventManagementService eventManagementService) : PageModel
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

		public async Task<IActionResult> OnPostUpdateEventAsync(List<int> selectedMemberIds, string? editOption, List<string> selectedDays)
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
			eventToUpdate.EventCategoryColor = ViewModel?.EventDetails?.EventCategoryColor ?? EventCategoryColor.None;

			await _eventManagementService.UpdateEventAsync(eventToUpdate, selectedMemberIds, editOption, ViewModel.StartDate, ViewModel.EndDate, ViewModel.NewDate, selectedDays);
			ViewModel.SelectedDays = selectedDays;


			if (!eventToUpdate.EventMemberDates.Any(e => e.Date == ViewModel.Day) && editOption == "all")
			{
				return RedirectToPage("./CalendarOverview", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
			}

			return RedirectToPage("./EventDetails", new { eventId = eventToUpdate.Id, memberId = ViewModel.MemberId, day = ViewModel.NewDate.Year != 1 ? ViewModel.NewDate : ViewModel.Day });
		}

		public async Task<IActionResult> OnPostDeleteEventAsync(List<int> selectedMemberIds, string? deleteOption)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			await _eventManagementService.DeleteEventAsync(ViewModel.EventId, ViewModel.MemberId, ViewModel.Day, selectedMemberIds, deleteOption);

			return RedirectToPage("./CalendarOverview", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
		}
		public async Task<IActionResult> OnPostRouteToIndexAsync()
		{
			return RedirectToPage("./CalendarOverview", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
		}


	}
}
