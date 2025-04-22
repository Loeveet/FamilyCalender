using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Web.ViewModels
{
	public class InviteViewModel
	{
		public Guid InviteId { get; set; }
		public Calendar Calendar { get; set; }
		public string LoginRedirectUrl { get; set; }
		public bool RequireLoginOrCreateAccount => !string.IsNullOrWhiteSpace(LoginRedirectUrl);
        public bool UserIsOwner { get; set; }
    }
}
