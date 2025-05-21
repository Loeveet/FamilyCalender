using FamilyCalender.Web.ViewModels;

namespace FamilyCalender.Web.News
{
    public class NewsService
    {
        private readonly List<NewsItemViewModel> AllNews = new()
    {
        new ()
        {
            Id = "news_2025_05_21",
            Title = "Ny funktion!",
            Body = "I inställningar kan du nu välja mellan månadsvy och veckovy!",
            StartDate = new DateTime(2025, 5, 21),
            DurationDays = 14
        },
                new ()
        {
            Id = "news_2025_05_22",
            Title = "Ny funktion igen!!",
            Body = "Du som har fler än en kalender kan nu välja vilken du ska se i första hand!",
            StartDate = new DateTime(2025, 5, 21),
            DurationDays = 14
        }
        // Lägg till fler nyheter här vid behov
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
