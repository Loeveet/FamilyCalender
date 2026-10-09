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
                return NotFound();

            var culture = new CultureInfo("sv-SE");

            ViewModel.Member = await _eventManagementService.GetMemberAsync(memberId);
            ViewModel.Day = day;
            ViewModel.Members = await _eventManagementService.GetMembersForCalendarAsync(ViewModel.EventDetails.CalendarId);

            ViewModel.ChosenMembers = ViewModel.EventDetails.EventMemberDates
                .Select(x => x.Member)
                .Where(m => m != null)
                .Distinct()
                .ToList()!;

            ViewModel.SelectedMemberIds = ViewModel.EventDetails.EventMemberDates
                .Select(x => x.MemberId)
                .Distinct()
                .ToList();

            var orderedDates = ViewModel.EventDetails.EventMemberDates
                .Select(x => x.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            ViewModel.IsSingleEvent = orderedDates.Count == 1;
            ViewModel.NewDate = ViewModel.IsSingleEvent ? orderedDates.First().Date : null;
            if (ViewModel.IsSingleEvent)
            {
                ViewModel.Day = orderedDates.First().Date;
            }

            ViewModel.IsSingleMember = ViewModel.EventDetails.EventMemberDates
                .Select(x => x.Member)
                .Distinct()
                .Count() < 2;

            ViewModel.FormattedDate = ViewModel.IsSingleEvent
                ? orderedDates.First().ToString("yyyy-MM-dd")
                : string.Empty;

            ViewModel.FormattedInterval = !ViewModel.IsSingleEvent
                ? $"{orderedDates.First():yyyy-MM-dd} – {orderedDates.Last():yyyy-MM-dd}"
                : string.Empty;

            var allWeekdays = orderedDates
                .Select(d => culture.DateTimeFormat.GetDayName(d.DayOfWeek).ToLower())
                .ToList();

            ViewModel.SelectedDays = allWeekdays;

            var weekdayCounts = allWeekdays
                .GroupBy(d => d)
                .ToDictionary(g => g.Key, g => g.Count());

            var firstDate = orderedDates.First();
            var weekOrderFromFirst = Enumerable.Range(0, 7)
                .Select(i => culture.DateTimeFormat.GetDayName(firstDate.AddDays(i).DayOfWeek).ToLower())
                .ToList();

            ViewModel.WeekOrderFromFirstDate = weekOrderFromFirst;

            return Page();
        }




        public async Task<IActionResult> OnPostUpdateEventAsync()
        {
            var eventToUpdate = await _eventManagementService.GetEventDetailsAsync(ViewModel.EventId);
            if (eventToUpdate == null)
            {
                return NotFound();
            }

            // Determine the scope from saved dates, not the repetition label or posted fields.
            var isSingleDate = eventToUpdate.EventMemberDates
                .Select(x => x.Date.Date).Distinct().Count() == 1;
            var day = ViewModel.Day;
            if (isSingleDate)
            {
                const string dateKey = "ViewModel.NewDate";
                if (!ViewModel.NewDate.HasValue ||
                    (ModelState.TryGetValue(dateKey, out var dateState) && dateState.Errors.Count > 0))
                {
                    ModelState.AddModelError(dateKey, "Ange ett giltigt datum.");
                    var submitted = ViewModel;
                    ViewModel = new EventDetailsViewModel();
                    var result = await OnGetAsync(eventToUpdate.Id, submitted.MemberId, submitted.Day);
                    ViewModel.NewTitle = submitted.NewTitle;
                    ViewModel.NewDate = submitted.NewDate;
                    ViewModel.SelectedMemberIds = submitted.SelectedMemberIds;
                    ViewModel.EventDetails!.Text = submitted.EventDetails?.Text ?? "";
                    ViewModel.EventDetails.EventTime = submitted.EventDetails?.EventTime ?? "";
                    ViewModel.EventDetails.EventStopTime = submitted.EventDetails?.EventStopTime ?? "";
                    ViewModel.EventDetails.EventCategoryColor = submitted.EventDetails?.EventCategoryColor ?? EventCategoryColor.None;
                    ViewData["ShowEditEventModal"] = true;
                    return result;
                }

                day = ViewModel.NewDate.Value.Date;
            }

            eventToUpdate.Title = ViewModel.NewTitle;
            eventToUpdate.Text = ViewModel?.EventDetails?.Text ?? "";
            eventToUpdate.EventTime = ViewModel?.EventDetails?.EventTime ?? "";
            eventToUpdate.EventStopTime = ViewModel?.EventDetails?.EventStopTime ?? "";
            eventToUpdate.EventCategoryColor = ViewModel?.EventDetails?.EventCategoryColor ?? EventCategoryColor.None;

            if (isSingleDate)
            {
                if (!eventToUpdate.EventMemberDates.Any(x => x.MemberId == ViewModel.MemberId))
                {
                    return BadRequest("Personen tillhör inte den här händelsen.");
                }

                var selectedMemberIds = ViewModel.SelectedMemberIds
                    .Append(ViewModel.MemberId)
                    .ToHashSet();

                var calendarMembers = await _eventManagementService
                    .GetMembersForCalendarAsync(eventToUpdate.CalendarId);

                if (!selectedMemberIds.IsSubsetOf(calendarMembers.Select(x => x.Id)))
                {
                    return BadRequest("En vald person tillhör inte kalendern.");
                }

                foreach (var row in eventToUpdate.EventMemberDates.ToList())
                {
                    if (selectedMemberIds.Contains(row.MemberId))
                    {
                        row.Date = day;
                    }
                    else
                    {
                        eventToUpdate.EventMemberDates.Remove(row);
                    }
                }

                var newMemberIds = selectedMemberIds
                    .Except(eventToUpdate.EventMemberDates.Select(x => x.MemberId))
                    .ToList();

                foreach (var memberId in newMemberIds)
                {
                    eventToUpdate.EventMemberDates.Add(new EventMemberDate
                    {
                        EventId = eventToUpdate.Id,
                        MemberId = memberId,
                        Date = day
                    });
                }
            }

            await _eventManagementService.UpdateEventAsync(eventToUpdate);

            await pushNotificationService.SendPush(eventToUpdate, false, await GetCurrentUserAsync());

            return RedirectToPage("./EventDetails", new
            {
                eventId = eventToUpdate.Id,
                memberId = ViewModel.MemberId,
                day
            });
        }

        public async Task<IActionResult> OnPostDeleteEventAsync(List<int> selectedMemberIds, string? deleteOption)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var eventFromDb = await _eventManagementService.GetEventDetailsAsync(ViewModel.EventId);

            await _eventManagementService.DeleteEventAsync(ViewModel.EventId, ViewModel.MemberId, ViewModel.Day, selectedMemberIds, deleteOption);
            await pushNotificationService.SendPush(eventFromDb, true, await GetCurrentUserAsync());

            return RedirectToPage("./CalendarOverview", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
        }
        public async Task<IActionResult> OnPostRouteToIndexAsync()
        {
            return RedirectToPage("./CalendarOverview", new { year = ViewModel.Day.Year, month = ViewModel.Day.Month, calendarId = ViewModel.CalendarId });
        }


    }
}
