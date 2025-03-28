using System.CodeDom;
using FamilyCalender.Core.Interfaces;
using FamilyCalender.Core.Models.Entities;
using FamilyCalender.Core.Models.ViewModels;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FamilyCalender.Web.Pages
{
    public class InviteModel(InviteService inviteService, IAuthService authService) : BasePageModel(authService)
	{
	    private readonly InviteService _inviteService = inviteService;

		[BindProperty]
		public InviteViewModel ViewModel { get; set; } = new InviteViewModel();

		public async Task<IActionResult> OnGetAsync(Guid inviteId)
	    {
		    var calendar = await _inviteService.GetByInviteId(inviteId);
			ViewModel.Calendar = calendar;
			ViewModel.InviteId = inviteId;

		    return Page();
		}

        [HttpPost]
        public async Task<IActionResult> OnPostJoinEventAsync()
        {
			var user = await GetCurrentUserAsync();

			_inviteService.Join(ViewModel.InviteId, user.Id);

            return RedirectToPage("./Index");
        }

    }
}
