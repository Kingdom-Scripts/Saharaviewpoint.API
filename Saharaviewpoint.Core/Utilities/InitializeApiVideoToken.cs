// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using LazyCache;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Saharaviewpoint.Core.Contants;
using Saharaviewpoint.Models.ApiVideo.Response;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Constants;
using Serilog;
using System.Text;

namespace Saharaviewpoint.Core.Utilities;
public static class InitializeApiVideoToken
{
    public static async Task<bool> InitializeToken(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var cacheService = serviceScope.ServiceProvider.GetService<IAppCache>();
        if (cacheService == null) throw new ArgumentNullException(nameof(cacheService));

        var httpClientFactory = serviceScope.ServiceProvider.GetService<IHttpClientFactory>();
        if (httpClientFactory == null) throw new ArgumentNullException(nameof(httpClientFactory));

        var httpClient = httpClientFactory.CreateClient(HttpClientKeys.ApiVideo);

        var appConfig = serviceScope.ServiceProvider.GetService<IOptions<AppConfig>>();
        string apiVideoKey = appConfig!.Value.ApiVideo.Key;

        var request = new { apiKey = apiVideoKey };
        var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync("auth/api-key", content);

        if (!response.IsSuccessStatusCode)
        {
            string stringResponse = await response.Content.ReadAsStringAsync();
            object error = JsonConvert.DeserializeObject<object>(stringResponse);
            Log.Error("--> Could not get Api.Video Token: {@Error}", error ?? "Unknown error");
            return false;
        }

        string resString = await response.Content.ReadAsStringAsync();
        var tokenObj = JsonConvert.DeserializeObject<ApiVideoAuthView>(resString);
        string token = tokenObj.access_token;
        string refreshToken = tokenObj.refresh_token;

        cacheService.Remove(AuthKeys.ApiVideoToken);
        cacheService.Remove(AuthKeys.ApiVideoRefreshToken);
        cacheService.Add(AuthKeys.ApiVideoToken, token, DateTime.UtcNow.AddSeconds(3590));
        cacheService.Add(AuthKeys.ApiVideoRefreshToken, refreshToken, DateTime.UtcNow.AddYears(20));

        Log.Information("--> Api.Video Token initialized");
        return true;
    }
}