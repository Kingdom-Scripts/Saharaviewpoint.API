// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using LazyCache;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Saharaviewpoint.Models.ApiVideo.Response;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Constants;
using Serilog;
using System.Net.Http.Headers;
using System.Text;

namespace Saharaviewpoint.Core.Middlewares;
internal class ApiVideoHttpHandler : DelegatingHandler
{
    private readonly string _apiVideoBaseUrl;
    private readonly string _apiVideoKey;
    private readonly IAppCache _cacheService;
    private readonly ILogger _logger;

    public ApiVideoHttpHandler(IOptions<AppConfig> appConfig, IAppCache cacheService, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(appConfig);
        _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _apiVideoBaseUrl = appConfig.Value.ApiVideo.BaseUrl;
        _apiVideoKey = appConfig.Value.ApiVideo.Key;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // get the current url
        string requestedUrl = request.RequestUri.ToString();
        if (!requestedUrl.Contains("api-key"))
        {
            var bearerToken = await GetBearerToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken ?? "");
        }

        var response = await base.SendAsync(request, cancellationToken);
        return response;
    }

    private async Task<string> GetBearerToken()
    {
        // get token
        string token = await _cacheService.GetAsync<string>(AuthKeys.ApiVideoToken);
        if (!string.IsNullOrEmpty(token)) return token;

        string refreshToken = await _cacheService.GetAsync<string>(AuthKeys.ApiVideoRefreshToken);

        ApiVideoAuthView response;
        if (!string.IsNullOrEmpty(refreshToken))
        {
            response = await RefreshToken(refreshToken);
            _logger.Information($"--> Api.Video Token refreshed on {DateTime.UtcNow}");
        }
        else
        {
            _logger.Information($"--> Api.Video Token created on {DateTime.UtcNow}");
            response = await CreateNewToken();
        }

        DateTime now = DateTime.UtcNow;
        _cacheService.Remove(AuthKeys.ApiVideoToken);
        _cacheService.Remove(AuthKeys.ApiVideoRefreshToken);
        _cacheService.Add(AuthKeys.ApiVideoToken, response.access_token, now.AddSeconds(3590));
        _cacheService.Add(AuthKeys.ApiVideoRefreshToken, response.refresh_token, now.AddDays(30));

        return response.access_token;
    }

    private async Task<ApiVideoAuthView> RefreshToken(string refreshToken)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_apiVideoBaseUrl)
        };

        var content = new StringContent(JsonConvert.SerializeObject(new { refreshToken }), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("auth/refresh", content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error($"--> Could not refresh Api.Video Token: {response.StatusCode}");
            return await CreateNewToken();
        }

        string resStri = await response.Content.ReadAsStringAsync();
        var res = JsonConvert.DeserializeObject<ApiVideoAuthView>(resStri);

        return res;
    }

    private async Task<ApiVideoAuthView> CreateNewToken()
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(_apiVideoBaseUrl)
        };

        var content = new StringContent(JsonConvert.SerializeObject(new { apiKey = _apiVideoKey }), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync("auth/api-key", content);

        if (!response.IsSuccessStatusCode)
        {
            _logger.Error($"--> Could not get Api.Video Token: {response.StatusCode}");
            throw new Exception("Request to third-party service failed.");
        }

        string resStri = await response.Content.ReadAsStringAsync();
        var res = JsonConvert.DeserializeObject<ApiVideoAuthView>(resStri);

        return res;
    }
}
