using System.ComponentModel.DataAnnotations;

namespace FamilyCalender.Core.Models.Entities
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int CalendarId { get; set; }
        public Calendar? Calendar { get; set; }
        public ICollection<EventMemberDate> EventMemberDates { get; set; } = [];
        public EventCategoryColor EventCategoryColor { get; set; }
		public RepeatType? RepeatIntervalType { get; set; }
		public int? CustomIntervalInWeeks { get; set; }      // Endast relevant om RepeatType == Custom
		public string EventTime { get; set; }
        public string EventStopTime { get; set; }
        public DateTime? LastEditedUtc { get; set; }
        public DateTime? CreatedUtc { get; set; }
    }

    public enum EventCategoryColor
    {
        None = 0,
        Blue = 10,
        Green = 20,
        Yellow = 30,
        Pink = 40,
        Red = 50,
        Purple = 60
    }
	public enum RepeatType
	{
		[Display(Name = "Upprepas inte")]
		None = 0,

		[Display(Name = "Varje dag")]
		Daily = 10,

		[Display(Name = "Varje vecka")]
		Weekly = 20,

		[Display(Name = "Varannan vecka")]
		BiWeekly = 30,

		[Display(Name = "Varje månad")]
		Monthly = 40,

		[Display(Name = "Varje år")]
		Yearly = 50,

		[Display(Name = "Anpassad")]
		Custom = 60
	}
}
