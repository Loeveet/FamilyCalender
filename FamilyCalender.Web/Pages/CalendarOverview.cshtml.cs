using Microsoft.AspNetCore.Mvc;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Web.ViewModels;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Web.Code;
using System.Globalization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static FamilyCalender.Web.ViewModels.CalendarOverViewViewModel;
using FamilyCalender.Web.News;
using System.Net;


namespace FamilyCalender.Web.Pages
{
    public class CalendarOverviewModel(
            CalendarManagementService calendarManagementService,
            PublicHolidayService publicHolidayService,
            PushNotificationService pushNotificationService,
            NewsService newsService,
            ICalendarAccessService calendarAccessService,
            IUserService userService,
            IAuthService authService) : BasePageModel(authService)
    {
        private readonly CalendarManagementService _calendarManagementService = calendarManagementService;
        private readonly NewsService _newsService = newsService;
        private readonly ICalendarAccessService _calendarAccessService = calendarAccessService;
        private readonly IUserService _userService = userService;
        [BindProperty]
        public CalendarOverViewViewModel ViewModel { get; set; } = new CalendarOverViewViewModel();
        public List<NewsItemViewModel> NewsItemToShow { get; set; } = [];

        public async Task<IActionResult> OnGetAsync(int? year, int? month, int? calendarId, string? view, int? weekOffset, DateTime? weekDate)
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return RedirectToPage("/Login");
            }
            await _authService.SetLastLoggedInAsync(user);
            ViewModel.CurrentUserAllowsPush = user.NotificationSetting is { AllowNotifications: true };

            var cookieValue = Request.Cookies["dismissed_news"];

			if (!string.IsNullOrEmpty(cookieValue))
			{
				var decodedValue = WebUtility.UrlDecode(cookieValue);
				var dismissedIds = decodedValue
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.Select(id => id.Trim())
					.ToList();

				NewsItemToShow = _newsService.GetCurrentNews(dismissedIds);
			}
			else
			{
				NewsItemToShow = _newsService.GetCurrentNews(new List<string>());
			}

			var calendars = await _calendarManagementService.GetCalendarDtosForUserAsync(user.Id);
            if (!calendarId.HasValue)
            {
                var preferredCalendarId = await _userService.GetPreferredCalendarIdAsync(user.Id);

                if (preferredCalendarId != null && calendars.Any(c => c.Id == preferredCalendarId))
                {
                    calendarId = preferredCalendarId;
                }
                else
                {
                    calendarId = calendars[0].Id;
                }

            }


            var calendarAccess = await _calendarAccessService.GetCalendarAccessAsync(user.Id, calendarId!.Value);
            if (calendarAccess?.Settings != null)
            {
                ViewModel.SelectedView = calendarAccess.Settings.PreferWeekView
                                            ? CalendarView.Week
                                            : CalendarView.Month;
            }

            SetCurrentYearAndMonth(year, month);
            var publicHolidays = publicHolidayService.GetHolidays(ViewModel.CurrentYear);

            if (ViewModel.SelectedView == CalendarView.Week)
            {
                var dateToShow = weekDate ?? DateTime.Today;

                ViewModel.CurrentWeekOfYear = GetIso8601WeekOfYear(DateTime.Today);
                ViewModel.DisplayedWeekOfYear = GetIso8601WeekOfYear(dateToShow);

                ViewModel.DaysInMonth = GenerateWeekDays(dateToShow, ViewModel.CultureInfo, publicHolidays);

                var calendarDtos = await _calendarManagementService.GetCalendarDtosForUserAsync(user.Id);
                ViewModel.CalendarDtos = calendarDtos;
                var calendarIds = calendarDtos.Select(c => c.Id).ToList();

                await LoadWeekViewData(calendarId, calendarIds, dateToShow, publicHolidays);
            }
            else
            {
                ViewModel.DaysInMonth = GenerateMonthDays(ViewModel.CurrentYear, ViewModel.CurrentMonth, ViewModel.CultureInfo, publicHolidays);

                var calendarDtos = await _calendarManagementService.GetCalendarDtosForUserAsync(user.Id);
                ViewModel.CalendarDtos = calendarDtos;
                var calendarIds = calendarDtos.Select(c => c.Id).ToList();

                await LoadMonthViewData(calendarId, calendarIds, publicHolidays);
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
            if (eventMemberDates == null || eventMemberDates.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Ange ett giltigt datum eller intervall.");
                return Page();
            }

            var evt = new NewCalendarEventSaveModel()
            {
                Title = ViewModel.EventTitle,
                Text = ViewModel.EventText,
                EventTime = ViewModel.EventTime,
                EventStopTime = ViewModel.EventStopTime,
                EventCategoryColor = ViewModel.SelectedCategoryColor,
                EventMemberDates = eventMemberDates,
                CalendarId = ViewModel.SelectedCalendarId,
                MemberIds = selectedMemberIds,
                RepeatIntervalType = ViewModel.RepetitionType,
                CustomIntervalInWeeks = ViewModel.RepetitionType == RepeatType.Custom ? intervalInWeeks : null
            };
            await _calendarManagementService.CreateEventAsync(evt);
            //await _calendarManagementService.CreateEventAsync(
            //	ViewModel.EventTitle,
            //	ViewModel.EventText ?? "",
            //	ViewModel.EventTime ?? "",
            //	ViewModel.EventStopTime ?? "",
            //	ViewModel.SelectedCategoryColor,
            //	eventMemberDates,
            //	ViewModel.SelectedCalendarId,
            //	selectedMemberIds);

            var user = await GetCurrentUserAsync();

            //var users = await _calendarManagementService.GetPushSubscribers(ViewModel.SelectedCalendarId, user.Id);
            await pushNotificationService.SendPush(evt, user);

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
        private async Task LoadWeekViewData(int? calendarId, List<int> calendarIds, DateTime weekDate, List<PublicHolidayInfo> publicHolidays)
        {
            // Sätt startdatum för veckan baserat på year, month och weekOffset
            // Hämta events och andra data för veckan
            ViewModel.DaysInMonth = GenerateWeekDays(weekDate, ViewModel.CultureInfo, publicHolidays);

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
        }
        private async Task LoadMonthViewData(int? calendarId, List<int> calendarIds, List<PublicHolidayInfo> publicHolidays)
        {
            ViewModel.DaysInMonth = GenerateMonthDays(ViewModel.CurrentYear, ViewModel.CurrentMonth, ViewModel.CultureInfo, publicHolidays);

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
            if (!ViewModel.SelectedDate.HasValue)
                return null;

            var startDate = ViewModel.SelectedDate.Value;

            if (ViewModel.RepetitionType == RepeatType.None)
            {
                return new List<EventMemberDate>
                {
                    new() { Date = startDate }
                };
            }

            if (!ViewModel.EndDate.HasValue)
                return null;

            var endDate = ViewModel.EndDate.Value;

            return ViewModel.RepetitionType switch
            {
                RepeatType.Daily => GenerateDailyEventDates(startDate, endDate),
                RepeatType.Weekly => GenerateWeeklyEventDates(startDate, endDate, 1),
                RepeatType.BiWeekly => GenerateWeeklyEventDates(startDate, endDate, 2),
                RepeatType.Monthly => GenerateMonthlyEventDates(startDate, endDate),
                RepeatType.Yearly => GenerateYearlyEventDates(startDate, endDate),
                RepeatType.Custom => GenerateEventMemberDatesInRangeWithWeekdays(startDate, endDate, selectedDays, intervalInWeeks),
                _ => null
            };
        }
        private List<EventMemberDate> GenerateDailyEventDates(DateTime startDate, DateTime endDate)
        {
            var dates = new List<EventMemberDate>();
            for (var current = startDate; current <= endDate; current = current.AddDays(1))
            {
                dates.Add(new EventMemberDate { Date = current });
            }
            return dates;
        }
        private List<EventMemberDate> GenerateWeeklyEventDates(DateTime start, DateTime end, int intervalInWeeks)
        {
            var result = new List<EventMemberDate>();

            for (var date = start; date <= end; date = date.AddDays(7 * intervalInWeeks))
            {
                result.Add(new EventMemberDate { Date = date });
            }

            return result;
        }
        private List<EventMemberDate> GenerateMonthlyEventDates(DateTime startDate, DateTime endDate)
        {
            var dates = new List<EventMemberDate>();

            for (var current = startDate; current <= endDate; current = current.AddMonths(1))
            {
                dates.Add(new EventMemberDate { Date = current });
            }

            return dates;
        }

        private List<EventMemberDate> GenerateYearlyEventDates(DateTime startDate, DateTime endDate)
        {
            var dates = new List<EventMemberDate>();

            for (var current = startDate; current <= endDate; current = current.AddYears(1))
            {
                dates.Add(new EventMemberDate { Date = current });
            }

            return dates;
        }


        public static List<DayViewModel> GenerateMonthDays(int year, int month, CultureInfo cultureInfo, List<PublicHolidayInfo> publicHolidays)
        {
            var daysCount = DateTime.DaysInMonth(year, month);
            var days = new List<DayViewModel>();

            for (var day = 1; day <= daysCount; day++)
            {
                var date = new DateTime(year, month, day);
                var weekOfYear = GetIso8601WeekOfYear(date);
                var dayName = date.ToString("dddd", cultureInfo);

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

            var totalDays = (end - start).Days;
            var startWeekNumber = GetIso8601WeekOfYear(start);

            for (int i = 0; i <= totalDays; i++)
            {
                var currentDate = start.AddDays(i);
                var dayName = culture.DateTimeFormat.GetDayName(currentDate.DayOfWeek).ToLower();
                var currentWeekNumber = GetIso8601WeekOfYear(currentDate);
                var weekDifference = currentWeekNumber - startWeekNumber;

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

            var weekNumber = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday
            );

            if (weekNumber == 1 && date.Month == 12)
            {
                return 53;
            }

            return weekNumber;
        }
        public static List<DayViewModel> GenerateWeekDays(DateTime referenceDate, CultureInfo cultureInfo, List<PublicHolidayInfo> publicHolidays)
        {
            var monday = FirstDateOfWeekISO8601(referenceDate.Year, GetIso8601WeekOfYear(referenceDate));

            var days = new List<DayViewModel>();
            for (int i = 0; i < 7; i++)
            {
                var day = monday.AddDays(i);
                string dayName = day.ToString("dddd", cultureInfo);

                days.Add(new DayViewModel
                {
                    Date = day,
                    IsCurrentDay = day.Date == DateTime.Today,
                    IsPastDay = day.Date < DateTime.Today,
                    WeekOfYear = GetIso8601WeekOfYear(day),
                    ShowWeekNumber = day.DayOfWeek == DayOfWeek.Monday,
                    CapitalizedDayName = char.ToUpper(dayName[0]) + dayName.Substring(1),
                    PublicHoliday = publicHolidays.FirstOrDefault(x => x.DateTime.Date == day.Date)
                });
            }

            return days;
        }


        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            var jan1 = new DateTime(year, 1, 1);
            var daysOffset = DayOfWeek.Monday - jan1.DayOfWeek;
            var firstMonday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            var firstWeek = GetIso8601WeekOfYear(jan1);

            if (firstWeek <= 1)
            {
                weekOfYear -= 1;
            }

            return firstMonday.AddDays(weekOfYear * 7);
        }
        public async Task<IActionResult> OnPostSetPreferredCalendarAsync(int calendarId)
        {
            var userId = await GetCurrentUserIdAsync();
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            await _userService.UpdatePreferredCalendarIdAsync(userId.Value, calendarId);

            return RedirectToPage(new { calendarId });
        }

    }
}