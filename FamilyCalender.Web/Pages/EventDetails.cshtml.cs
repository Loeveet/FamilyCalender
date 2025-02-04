using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Core.Models;
using FamilyCalender.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NuGet.Packaging;
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

			ViewModel.Member = await _eventManagementService.GetMemberAsync(memberId);
			ViewModel.Day = day;
			ViewModel.Members = await _eventManagementService.GetMembersForCalendarAsync(ViewModel.EventDetails.CalendarId);

			var selectedDays = ViewModel.EventDetails.EventMemberDates
				.Where(ed => ed.Date >= DateTime.Today)
				.Select(ed => ed.Date.DayOfWeek)
				.Distinct()
				.ToList();

			ViewModel.SelectedDays = selectedDays;

			return Page();
		}

		public async Task<IActionResult> OnPostUpdateEventAsync(List<int> selectedMemberIds, string? editOption, List<DayOfWeek> selectedDays)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			var eventToUpdate = await _eventManagementService.GetEventDetailsAsync(ViewModel.EventId);
			if (eventToUpdate == null)
			{
				return NotFound();
			}

			eventToUpdate.Title = ViewModel.NewTitle;

			await _eventManagementService.UpdateEventAsync(eventToUpdate, selectedMemberIds, editOption, ViewModel.StartDate, ViewModel.EndDate, ViewModel.NewDate, selectedDays);

			if (!eventToUpdate.EventMemberDates.Any(e => e.Date == ViewModel.Day) && editOption == "all")
			{
				return RedirectToPage("./Index", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
			}

			return RedirectToPage("./EventDetails", new { eventId = eventToUpdate.Id, memberId = ViewModel.Member.Id, day = ViewModel.NewDate });
		}

		public async Task<IActionResult> OnPostDeleteEventAsync(List<int> selectedMemberIds, string? deleteOption)
		{
			if (!ModelState.IsValid)
			{
				return Page();
			}

			await _eventManagementService.DeleteEventAsync(ViewModel.EventId, ViewModel.MemberId, ViewModel.Day, selectedMemberIds, deleteOption);

			return RedirectToPage("./Index", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
		}

	}
}
