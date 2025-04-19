using Microsoft.AspNetCore.Mvc;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Core.Models.ViewModels;
using FamilyCalender.Core.Interfaces;

namespace FamilyCalender.Web.Pages
{
    public class IndexModel(
			CalendarManagementService calendarManagementService,
			IAuthService authService) : BasePageModel(authService)
	{
		private readonly CalendarManagementService _calendarManagementService = calendarManagementService;

		[BindProperty]
		public IndexViewModel ViewModel { get; set; } = new IndexViewModel();

		public async Task<IActionResult> OnGetAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                ViewModel.DaysInMonth = CalendarManagementService.GenerateMonthDays(ViewModel.CurrentYear, ViewModel.CurrentMonth, ViewModel.CultureInfo);

                var calendarDtos = await _calendarManagementService.GetCalendarDtosForUserAsync(user.Id);
                ViewModel.CalendarDtos = calendarDtos;
            }
           
			return Page();
		}
	}
}
