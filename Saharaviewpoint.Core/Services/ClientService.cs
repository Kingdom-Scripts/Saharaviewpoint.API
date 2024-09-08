// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using LazyCache;
using Microsoft.EntityFrameworkCore;
using Saharaviewpoint.Core.Extensions;
using Saharaviewpoint.Core.Interfaces;
using Saharaviewpoint.Core.Utilities;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Input.Client;
using Saharaviewpoint.Models.Utilities;
using Saharaviewpoint.Models.View.Client;

namespace Saharaviewpoint.Core.Services;

// TODO: add caching
public class ClientService(SaharaviewpointContext context, IAppCache cache) : BaseService, IClientService
{
    private readonly SaharaviewpointContext _context = context ?? throw new ArgumentNullException(nameof(context));
    private readonly IAppCache _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public async Task<Result> ListClients(ClientSearchModel request)
    {
        string generatedKey = GenerateCacheKey(request);
        string cacheKey = CacheKeys.ListClients() + generatedKey;

        // Retrieve the current list of cache keys and add the new key
        var cacheKeys = _cache.GetOrAdd(CacheKeys.ListClients(), () => new List<string>(), new TimeSpan(0, 45, 0));
        if (!cacheKeys.Contains(cacheKey))
        {
            cacheKeys.Add(cacheKey);
            _cache.Add(CacheKeys.ListClients(), cacheKeys);
        }

        // Try to get the cached result
        var cachedResult = await _cache.GetOrAddAsync(cacheKey, async () =>
        {
            return await _context.Users
            .Where(u => u.Type == UserTypes.CLIENT)
            .Where(u => string.IsNullOrEmpty(request.SearchQuery) || u.FirstName.Contains(request.SearchQuery) || u.LastName.Contains(request.SearchQuery) || u.Email.Contains(request.SearchQuery))
            .Where(u => !request.DateJoinedStart.HasValue || u.CreatedAt >= request.DateJoinedStart.Value.ToUniversalTime())
            .Where(u => !request.DateJoinedEnd.HasValue || u.CreatedAt <= request.DateJoinedEnd.Value.ToUniversalTime())
            .Where(u => !request.IsActiveOnly || u.IsActive == true)
            .Where(u => !request.IsInactiveOnly || u.IsActive == false)
            .OrderBy(u => u.FirstName)
            .Select(u => new ClientView
            {
                Uid = u.Uid,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                //NoOfProjects = u.Projects.Where(p => p.CreatedById == u.Id).Count(),
                NoOfProjects = _context.Projects.Where(p => p.CreatedById == u.Id).Count(),
                IsActive = u.IsActive,
                JoinedOn = u.CreatedAt
            })
            .ToPaginatedListAsync(request.PageIndex, request.PageSize);
        }, new TimeSpan(0, 45, 0));

        return new SuccessResult(cachedResult);
    }

    public async Task<Result> DeactivateClient(string uid)
    {
        var client = await _context.Users.FirstOrDefaultAsync(u => u.Uid.ToString() == uid);
        if (client == null)
            return new ErrorResult("Client not found");

        client.IsActive = false;
        await _context.SaveChangesAsync();

        // clear caches
        _cache.ClearCaches(CacheKeys.ListClients());

        return new SuccessResult();
    }

    public async Task<Result> ActivateClient(string uid)
    {
        var client = await _context.Users.FirstOrDefaultAsync(u => u.Uid.ToString() == uid);
        if (client == null)
            return new ErrorResult("Client not found");

        client.IsActive = true;
        await _context.SaveChangesAsync();

        // clear caches
        _cache.ClearCaches(CacheKeys.ListClients());

        return new SuccessResult();
    }

    internal static class CacheKeys
    {
        internal static string ListClients() => "CLientService-ListClients";
    }
}