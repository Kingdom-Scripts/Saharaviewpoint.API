using LazyCache;

namespace Saharaviewpoint.Core.Utilities;
public static class CacheUtil
{
    public static void ClearListCache(this IAppCache cache, params string[] listKey)
    {
        foreach(string cacheKey in listKey)
        {
            var cacheKeys = cache.Get<List<string>>(cacheKey);
            if (cacheKeys != null)
            {
                foreach (var key in cacheKeys)
                {
                    cache.Remove(key);
                }
                cache.Remove(cacheKey);
            }
        }
    }
}
