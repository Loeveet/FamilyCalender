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
using System.Security.Claims;
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

		public Task<IActionResult> OnGetAsync()
		{
			
			return Task.FromResult<IActionResult>(Page());
		}
	}
}
