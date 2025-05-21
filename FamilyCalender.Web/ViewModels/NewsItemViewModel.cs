namespace FamilyCalender.Web.ViewModels
{
    public class NewsItemViewModel
    {
		public string Id { get; set; }
		public string Title { get; set; }
		public string Body { get; set; }
		public string Button { get; set; }
		public DateTime StartDate { get; set; }
		public int DurationDays { get; set; }
		public DateTime EndDate => StartDate.AddDays(DurationDays);
		public bool IsActive(DateTime now) => StartDate <= now && now <= EndDate;
	}
}
