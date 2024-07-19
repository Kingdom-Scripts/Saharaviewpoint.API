// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using LazyCache;

namespace Saharaviewpoint.Core.Utilities;
public static class CacheUtil
{
    public static void ClearCaches(this IAppCache cache, params string[] cacheKeys)
    {
        foreach (string key in cacheKeys)
        {
            // Attempt to get the key as if it points to a list of cache keys
            var listKeys = cache.Get<List<string>>(key);
            if (listKeys != null)
            {
                // If it does, clear each cache key in the list
                foreach (var listKey in listKeys)
                {
                    cache.Remove(listKey);
                }
            }
            // Whether it was a list of keys or a single key, remove the key itself
            cache.Remove(key);
        }
    }
}
