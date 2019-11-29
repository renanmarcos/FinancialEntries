using FinancialEntries.Services.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace FinancialEntries.Services.Observers
{
    public class CacheObserver : Observer
    {
        private CacheKey _cacheKey;

        private IMemoryCache _cache;

        public CacheObserver(CacheKey cacheKey)
        {
            _cacheKey = cacheKey;
            _cache = StaticServiceProvider.Provider
                        .GetService(typeof(IMemoryCache)) as IMemoryCache;
        }

        public void Notify()
        {
            _cache.Remove(_cacheKey);
        }
    }
}