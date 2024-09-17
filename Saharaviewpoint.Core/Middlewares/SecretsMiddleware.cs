// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Saharaviewpoint.Core.Contants;
using Saharaviewpoint.Core.Utilities;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Middlewares;

public class SecretsMiddleware
{
    private readonly RequestDelegate _next;
    private static bool _hasRun = false;
    private static readonly object _lock = new object();

    public SecretsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IOptions<AppConfig> appConfig, ScopedSecrets secrets)
    {
        if (!_hasRun)
        {
            lock (_lock)
            {
                if (!_hasRun)
                {
                    var keyVault = new KeyVaultUtil(appConfig.Value.KeyVaultUrl);
                    secrets.ApiVideoKey = keyVault.GetSecret(KeyVaultKeys.ApiVideoKey);
                    secrets.JwtSecert = keyVault.GetSecret(KeyVaultKeys.JwtSecert);
                    secrets.StorageKey = keyVault.GetSecret(KeyVaultKeys.StorageKey);
                    secrets.TinifyKey = keyVault.GetSecret(KeyVaultKeys.TinifyKey);
                    secrets.ZeptoLogoKey = keyVault.GetSecret(KeyVaultKeys.ZeptoMailLogoKey);

                    _hasRun = true;
                }
            }
        }

        // Call the next delegate/middleware in the pipeline
        await _next(context);
    }
}