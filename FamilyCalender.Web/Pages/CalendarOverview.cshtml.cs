using Microsoft.AspNetCore.Mvc;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Web.ViewModels;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Web.Code;
using System.Globalization;


namespace FamilyCalender.Web.Pages
{
    public class CalendarOverviewModel(
			CalendarManagementService calendarManagementService,
            PublicHolidayService publicHolidayService,
            IAuthService authService) : BasePageModel(authService)
	{
		private readonly CalendarManagementService _calendarManagementService = calendarManagementService;

		private static readonly string[] AllowedEmails = new[]
	 {
				"loeveet@gmail.com",
				"mikael.lennander@hotmail.com",
				"carolinaguevara@hotmail.com",
                "jenny.liliegren@outlook.com",
            };

		[BindProperty]
		public CalendarOverViewViewModel ViewModel { get; set; } = new CalendarOverViewViewModel();

		public async Task<IActionResult> OnGetAsync(int? year, int? month, int? calendarId)
		{
			var user = await GetCurrentUserAsync();
			if (user == null)
			{
				return RedirectToPage("/Login");
			}

			ViewModel.ShowUserSettings = AllowedEmails.Contains(user.Email);
			SetCurrentYearAndMonth(year, month);

            var publicHolidays = publicHolidayService.GetHolidays(ViewModel.CurrentYear);

			ViewModel.DaysInMonth = GenerateMonthDays(ViewModel.CurrentYear, ViewModel.CurrentMonth, ViewModel.CultureInfo, publicHolidays);

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

		public async Task<IActionResult> OnPostAddEventAsync(List<int> selectedMemberIds, List<string> selectedDays, int intervalInWeeks = 1)
		{
			if (!IsValidInput(selectedMemberIds))
			{
				ModelState.AddModelError(string.Empty, "Titel, datum, medlem och kalender är obligatoriskt.");
				return Page();
			}

			var eventMemberDates = ValidateAndGenerateEventMemberDates(selectedDays, intervalInWeeks);
			if (eventMemberDates == null)
			{
				ModelState.AddModelError(string.Empty, "Ange ett giltigt datum eller intervall.");
				return Page();
			}

			await _calendarManagementService.CreateEventAsync(
				ViewModel.EventTitle,
				ViewModel.EventText ?? "",
				ViewModel.EventTime ?? "",
				ViewModel.EventStopTime ?? "",
				ViewModel.SelectedCategoryColor,
				eventMemberDates,
				ViewModel.SelectedCalendarId,
				selectedMemberIds);

			var user = await GetCurrentUserAsync();

			//var users = await _calendarManagementService.GetPushSubscribers(ViewModel.SelectedCalendarId, user.Id);
			var users = await _calendarManagementService.GetPushSubscribers(ViewModel.SelectedCalendarId, -1); // so we always get push during beta
			foreach (var pushUser in users)
			{
				if (pushUser.NotificationSetting != null)
				{
					new PushNotificationService().SendPush(new PushData()
					{
						Title = $"{user.Email} Skapade {ViewModel.EventTitle}",
						Body = $"{eventMemberDates.FirstOrDefault().Date.ToString("yyyy-MM-dd")} {ViewModel.EventTime ?? ""} {ViewModel.EventText ?? ""}"
					},
					pushUser.Email,
					pushUser.NotificationSetting.Endpoint, pushUser.NotificationSetting.P256dh, pushUser.NotificationSetting.Auth);
				}
				
			}
			
				
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
				   selectedMemberIds != null &&
				   selectedMemberIds.Count != 0 &&
				   ViewModel.SelectedCalendarId > 0;
		}

		private List<EventMemberDate>? ValidateAndGenerateEventMemberDates(List<string> selectedDays, int intervalInWeeks)
		{
			if (ViewModel.StartDate.HasValue && ViewModel.EndDate.HasValue && selectedDays != null)
			{
				return GenerateEventMemberDatesInRangeWithWeekdays(
					ViewModel.StartDate.Value,
					ViewModel.EndDate.Value,
					selectedDays,
					intervalInWeeks);
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

		public static List<DayViewModel> GenerateMonthDays(int year, int month, CultureInfo cultureInfo, List<PublicHolidayInfo> publicHolidays)
		{
			var daysCount = DateTime.DaysInMonth(year, month);
			var days = new List<DayViewModel>();

			for (var day = 1; day <= daysCount; day++)
			{
				var date = new DateTime(year, month, day);
				var weekOfYear = GetIso8601WeekOfYear(date);
				string dayName = date.ToString("dddd", cultureInfo);

				days.Add(new DayViewModel
				{
					Date = date,
					IsCurrentDay = date.Date == DateTime.Today,
					IsPastDay = date.Date < DateTime.Today,
					WeekOfYear = weekOfYear,
					ShowWeekNumber = date.DayOfWeek == DayOfWeek.Monday || date.Day == 1,
					CapitalizedDayName = char.ToUpper(dayName[0]) + dayName.Substring(1),
					PublicHoliday = publicHolidays.FirstOrDefault(x => x.DateTime.Year == year && x.DateTime.Month == month && x.DateTime.Day == day)
				});
			}

			return days;
		}
		public static List<EventMemberDate> GenerateEventMemberDatesInRangeWithWeekdays(DateTime start, DateTime end, List<string> selectedDays, int intervalInWeeks)
		{
			var dates = new List<EventMemberDate>();
			var culture = new CultureInfo("sv-SE");

			var selectedDaysLower = selectedDays.Select(day => day.ToLower()).ToList();

			int totalDays = (end - start).Days;
			int startWeekNumber = GetIso8601WeekOfYear(start);

			for (int i = 0; i <= totalDays; i++)
			{
				var currentDate = start.AddDays(i);
				var dayName = culture.DateTimeFormat.GetDayName(currentDate.DayOfWeek).ToLower();
				int currentWeekNumber = GetIso8601WeekOfYear(currentDate);
				int weekDifference = currentWeekNumber - startWeekNumber;

				if (weekDifference % intervalInWeeks == 0 && selectedDaysLower.Contains(dayName))
				{
					dates.Add(new EventMemberDate { Date = currentDate });
				}
			}

			return dates;
		}


		public static int GetIso8601WeekOfYear(DateTime date)
		{
			var day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(date);

			if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
			{
				date = date.AddDays(3);
			}

			int weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
				date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday
			);

			if (weekNumber == 1 && date.Month == 12)
			{
				return 53;
			}

			return weekNumber;
		}
	}
}
