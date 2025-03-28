using FamilyCalender.Core.Models.Entities;

namespace FamilyCalender.Web.ViewModels
{
	public class InviteViewModel
	{
		public Guid InviteId { get; set; }
		public Calendar Calendar { get; set; }
	}
}
