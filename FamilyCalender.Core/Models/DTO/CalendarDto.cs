namespace FamilyCalender.Core.Models.Dto
{
	public class CalendarDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public Guid? InviteId { get; set; }
	}
}
