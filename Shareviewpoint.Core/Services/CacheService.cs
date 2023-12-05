using Shareviewpoint.Core.Interfaces;

namespace Shareviewpoint.Core.Services;

public class CacheService : ICacheService
{
    public void AddToken(string key, string value, DateTime expiresAt)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetToken(string key)
    {
        throw new NotImplementedException();
    }

    public void RemoveToken(string key)
    {
        throw new NotImplementedException();
    }
}