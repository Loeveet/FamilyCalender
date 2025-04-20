using Microsoft.Extensions.Caching.Memory;
using PublicHoliday;

namespace FamilyCalender.Web.Code
{
    public class PublicHolidayService
    {
        private readonly PublicHolidayBase _publicHoliday;
        private string _uniqueKey;
        private MemoryCache _Cache { get; } = new MemoryCache(
            new MemoryCacheOptions
            {
                
            });

        public PublicHolidayService(PublicHolidayBase publicHoliday, string country)
        {
            _publicHoliday = publicHoliday;
            _uniqueKey = $"PUBLIC-HOLIDAY-{country}";
           
        }

        public List<Core.Models.PublicHoliday> GetHolidays(int year)
        {
            var fromCache = GetFromCache<List<Core.Models.PublicHoliday>>(year);
            if (fromCache == null || !fromCache.Any())
            {
                var result = _publicHoliday.PublicHolidayNames(year)
                    .Select(c => new Core.Models.PublicHoliday
                    {
                        DateTime = c.Key,
                        HolidayName = c.Value
                    }).ToList();
                SaveToCache(year, result, DateTimeOffset.Now.AddHours(23));

                return result;
            }

            return fromCache;
        }
        


        public T? GetFromCache<T>(int year) where T : class
        {
            try
            {
                var cacheKey = GetCacheKey(year);
                var cacheItem = _Cache.Get<T>(cacheKey);

                return cacheItem;
            }
            catch
            {

            }

            return null;
        }

        public void SaveToCache(int year, object cacheObject, DateTimeOffset cacheExpirationTime)
        {
            try
            {
                var cacheKey = GetCacheKey(year);
                _Cache.Set(cacheKey, cacheObject, cacheExpirationTime);
            }
            catch
            {
            }
        }

        private string GetCacheKey(int year)
        {
            return string.Concat(_uniqueKey, year);
        }
    }
}
