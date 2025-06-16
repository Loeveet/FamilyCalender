using FamilyCalender.Web.ViewModels;

namespace FamilyCalender.Web.News
{
    public class NewsService
    {
        private readonly List<NewsItemViewModel> AllNews = new()
    {
        new ()
        {
            Id = "news_2025_05_21_1",
            Title = "Månadsvy eller veckovy?",
            Body = "I inställningar kan du nu välja mellan månadsvy och veckovy!",
            StartDate = new DateTime(2025, 5, 21),
            DurationDays = 14,
            Button = "Ah, jag förstår! 💃🕺"
        },
                new ()
        {
            Id = "news_2025_05_21_2",
            Title = "Gillar du att sidan scrollar ner till dagens datum?",
            Body = "I inställningar kan du nu välja om du vill att det ska scrollas eller inte!",
            StartDate = new DateTime(2025, 5, 21),
            DurationDays = 14,
            Button = "Kanon! 🤩"
        },
				new ()
		{
			Id = "news_2025_05_21_3",
			Title = "Flera kalendrar?",
			Body = "Den du var inne på senast är nu den som kommer visas nästa gång du kommer in på PlaneraMedFlera!",
			StartDate = new DateTime(2025, 5, 21),
			DurationDays = 14,
			Button = "Svinbra! 👌"
		},
				new ()
		{
			Id = "news_2025_06_06_1",
			Title = "Gemensamma listor!",
			Body = "Nu kan du även ha gemensamma listor kopplat till kalender. Det går att skapa båda att göra-listor samt checklistor, perfekt inför semestern!",
			StartDate = new DateTime(2025, 6, 5),
			DurationDays = 14,
			Button = "Check✅ ☀️⛱️"
		}
				,
				new ()
		{
			Id = "news_2025_06_16_1",
			Title = "Nytt alternativ för återkommande händelser!",
			Body = "Nu finns alternativet att få upprepade händelser var femte vecka. Perfekt om du jobbar femskift!",
			StartDate = new DateTime(2025, 6, 5),
			DurationDays = 14,
			Button = "OK! 👩‍🏭🏭 "
		}
        // Bara fylla på med nyheter här under. Eller ta bort sånt som inte är aktuellt längre. Nyheterna är aktiva i 14 dagar efter startDate
    };

		public List<NewsItemViewModel> GetCurrentNews(IEnumerable<string> dismissedNewsIds)
		{
			dismissedNewsIds ??= Enumerable.Empty<string>();

			return AllNews
				.Where(n => n.StartDate <= DateTime.Today
						 && n.EndDate >= DateTime.Today
						 && !dismissedNewsIds.Contains(n.Id))
				.ToList();
		}
	}
}
