using FamilyCalender.Core.Interfaces.IServices;
using FamilyCalender.Infrastructure.Services;
using FamilyCalender.Web.Code;
using FamilyCalender.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace FamilyCalender.Web.Pages
{
    public class InviteModel(InviteService inviteService, PushNotificationService pushNotificationService, IAuthService authService) : BasePageModel(authService)
	{
	    private readonly InviteService _inviteService = inviteService;

		[BindProperty]
		public InviteViewModel ViewModel { get; set; } = new InviteViewModel();

		public async Task<IActionResult> OnGetAsync(Guid inviteId)
	    {

            var user = await GetCurrentUserAsync();
            if (user == null)
            {
	            ViewModel.LoginRedirectUrl = $"/Login?ReturnUrl={HttpContext.Request.Path}";
	            //return Redirect($"/Login?ReturnUrl={HttpContext.Request.Path}");
            }

            var calendar = await _inviteService.GetByInviteId(inviteId);
            ViewModel.UserIsOwner = calendar.OwnerId == user?.Id && user?.Id != null;
			ViewModel.Calendar = calendar;
			ViewModel.InviteId = inviteId;

		    return Page();
		}

        [HttpPost]
        public async Task<IActionResult> OnPostJoinEventAsync()
        {
			var user = await GetCurrentUserAsync();

			_inviteService.Join(ViewModel.InviteId, user.Id);
			try
			{
				var calendar = await _inviteService.GetByInviteId(ViewModel.InviteId);
				if (calendar != null)
				{
					await pushNotificationService.SendAcceptCalendarInvitePush(calendar?.Id, calendar.Name, user.Email);
				}
			}
			catch (Exception e)
			{
				Log.Error("Error posting push to calendar owner during invite accept for calendar", e);
			}
			
			

            return RedirectToPage("./CalendarOverview");
        }

    }
}
