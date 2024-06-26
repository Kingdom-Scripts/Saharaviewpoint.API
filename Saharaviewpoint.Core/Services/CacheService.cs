using LazyCache;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Models.Constants;

namespace Saharaviewpoint.Core.Services;

public class CacheService(IAppCache cache) : ICacheService
{
    private readonly IAppCache _cache = cache;

    public void AddToken(string key, string token, DateTime expiresAt)
        => _cache.Add($"{AuthKeys.CacheKey}:{key}", token, expiresAt);

    public async Task<string> GetToken(string key)
        => await _cache.GetAsync<string>($"{AuthKeys.CacheKey}:{key}");

    public void RemoveToken(string key)
        => _cache.Remove($"{AuthKeys.CacheKey}:{key}");
}