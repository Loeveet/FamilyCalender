using Microsoft.AspNetCore.Mvc;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Core.Models.ViewModels;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Core.Interfaces;
using FamilyCalender.Web.Code;
using Serilog;
using Newtonsoft.Json;

namespace FamilyCalender.Web.Pages
{
    public class CalendarOverviewModel(
			CalendarManagementService calendarManagementService,
            PublicHolidayService publicHolidayService,
            IAuthService authService) : BasePageModel(authService)
	{
		private readonly CalendarManagementService _calendarManagementService = calendarManagementService;

		[BindProperty]
		public CalendarOverViewViewModel ViewModel { get; set; } = new CalendarOverViewViewModel();

		public async Task<IActionResult> OnGetAsync(int? year, int? month, int? calendarId)
		{
			var user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToPage("/Login");
			}
			SetCurrentYearAndMonth(year, month);

            var publicHolidays = publicHolidayService.GetHolidays(ViewModel.CurrentYear);

			ViewModel.DaysInMonth = CalendarManagementService.GenerateMonthDays(ViewModel.CurrentYear, ViewModel.CurrentMonth, ViewModel.CultureInfo, publicHolidays);

            var calendarDtos = await _calendarManagementService.GetCalendarDtosForUserAsync(user.Id);
			ViewModel.CalendarDtos = calendarDtos;
            var calendarIds = calendarDtos.Select(c => c.Id).ToList();


            if (calendarIds != null)
			{
				await LoadSelectedCalendarData(calendarId, calendarIds);
			}
			else
			{
				ViewModel.SelectedCalendar = new Core.Models.Entities.Calendar();
				ViewModel.Events = new List<Event>();
				ViewModel.Members = new List<Member>();
			}

			return Page();
		}

		public async Task<IActionResult> OnPostAddEventAsync(List<int> selectedMemberIds, List<string> selectedDays)
		{
			if (!IsValidInput(selectedMemberIds))
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
				ViewModel.EventText ?? "",
				ViewModel.SelectedCategoryColor,
				eventMemberDates,
				ViewModel.SelectedCalendarId,
				selectedMemberIds);

			return RedirectToPage("./CalendarOverview", new
			{
				year = ViewModel.CurrentYear,
				month = ViewModel.CurrentMonth,
				calendarId = ViewModel.SelectedCalendarId
			});
		}

		private async Task LoadSelectedCalendarData(int? calendarId, List<int> calendarIds)
		{
			var chosenCalendarId = calendarId.HasValue && calendarIds.Contains(calendarId.Value)
				? calendarId.Value
				: calendarIds.FirstOrDefault();

			if (chosenCalendarId == 0)
			{
				return;
			}


			var calendarDto = await _calendarManagementService.GetCalendarDtoByIdAsync(chosenCalendarId);
			if (calendarDto != null)
			{
				ViewModel.SelectedCalendarId = calendarDto.Id;
				ViewModel.SelectedCalendarName = calendarDto.Name;

				if (calendarDto.InviteId != null)
				{
					ViewModel.ShareLink = $"{Request.Scheme}://{Request.Host}/Invite?inviteId={calendarDto.InviteId}";
				}
			}


			ViewModel.Events = await _calendarManagementService.GetEventsForCalendarAsync(chosenCalendarId, ViewModel.CurrentYear, ViewModel.CurrentMonth);
			ViewModel.Members = await _calendarManagementService.GetMembersForCalendarAsync(chosenCalendarId);
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
