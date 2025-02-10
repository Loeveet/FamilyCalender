using System.Globalization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using FamilyCalender.Core.Interfaces.IServices;
using Microsoft.AspNetCore.Mvc;
using FamilyCalender.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using FamilyCalender.Core.Models.ViewModels;
using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Web.Pages
{
    public class IndexModel(
		UserManager<User> userManager,
		CalendarManagementService calendarManagementService) : PageModel
	{
		private readonly UserManager<User> _userManager = userManager;
		private readonly CalendarManagementService _calendarManagementService = calendarManagementService;

		[BindProperty]
		public IndexViewModel ViewModel { get; set; } = new IndexViewModel();

		public async Task<IActionResult> OnGetAsync(int? year, int? month, int? calendarId)
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return RedirectToPage("/Account/Login");
			}

			SetCurrentYearAndMonth(year, month);
			ViewModel.DaysInMonth = CalendarManagementService.GenerateMonthDays(ViewModel.CurrentYear, ViewModel.CurrentMonth, ViewModel.CultureInfo);

			ViewModel.Calendars = await _calendarManagementService.GetCalendarsForUserAsync(user.Id);


			if (ViewModel.Calendars != null && ViewModel.Calendars.Count > 0)
			{
				await LoadSelectedCalendarData(calendarId);
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAddEventAsync(List<int> selectedMemberIds, List<string> selectedDays)
		{
			if (!ModelState.IsValid || !IsValidInput(selectedMemberIds))
			{
				ModelState.AddModelError(string.Empty, "Titel, datum, medlem och kalender är obligatoriskt.");
				return Page();
			}

			var eventMemberDates = ValidateAndGenerateEventMemberDates(selectedDays);
			if (eventMemberDates == null)
			{
				ModelState.AddModelError(string.Empty, "Ange ett giltigt datum eller intervall.");
				return Page();
			}

			await _calendarManagementService.CreateEventAsync(
				ViewModel.EventTitle,
				eventMemberDates,
				ViewModel.SelectedCalendarId,
				selectedMemberIds);

			return RedirectToPage("./Index", new
			{
				year = ViewModel.CurrentYear,
				month = ViewModel.CurrentMonth,
				calendarId = ViewModel.SelectedCalendarId
			});
		}

		private async Task LoadSelectedCalendarData(int? calendarId)
		{
			var calendar = ViewModel.Calendars.FirstOrDefault(c => c.Id == calendarId);
			ViewModel.SelectedCalendar = calendar ?? ViewModel.Calendars.FirstOrDefault() ?? new Core.Models.Entities.Calendar();

			ViewModel.Events = await _calendarManagementService.GetEventsForCalendarAsync(ViewModel.SelectedCalendar.Id);
			ViewModel.Members = await _calendarManagementService.GetMembersForCalendarAsync(ViewModel.SelectedCalendar.Id);
		}

		private void SetCurrentYearAndMonth(int? year, int? month)
		{
			ViewModel.CurrentYear = year ?? ViewModel.CurrentYear;
			ViewModel.CurrentMonth = month ?? ViewModel.CurrentMonth;
		}

		private bool IsValidInput(List<int> selectedMemberIds)
		{
			return !string.IsNullOrEmpty(ViewModel.EventTitle) &&
				   ViewModel.SelectedDate.HasValue &&
				   ViewModel.SelectedDate.Value.Date >= DateTime.Now.Date &&
				   selectedMemberIds != null &&
				   selectedMemberIds.Count != 0 &&
				   ViewModel.SelectedCalendarId > 0;
		}

		private List<EventMemberDate>? ValidateAndGenerateEventMemberDates(List<string> selectedDays)
		{
			if (ViewModel.StartDate.HasValue && ViewModel.EndDate.HasValue && selectedDays != null)
			{
				return CalendarManagementService.GenerateEventMemberDatesInRangeWithWeekdays(
					ViewModel.StartDate.Value,
					ViewModel.EndDate.Value,
					selectedDays);
			}

			if (ViewModel.SelectedDate.HasValue)
			{
				return
				[
					new() { Date = ViewModel.SelectedDate.Value }
				];
			}

			return null;
		}
	}
}
