// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Saharaviewpoint.Core.Utilities;

public class KeyVaultUtil
{
    private readonly SecretClient _secretClient;

    public KeyVaultUtil(string keyVaultUrl)
    {
        SecretClientOptions options = new SecretClientOptions()
        {
            Retry = {
                Delay= TimeSpan.FromSeconds(2),
                MaxDelay = TimeSpan.FromSeconds(16),
                MaxRetries = 5,
                Mode = RetryMode.Exponential
            }
        };
        _secretClient = new SecretClient(new Uri(keyVaultUrl), new DefaultAzureCredential(), options);
    }

    public string GetSecret(string secretName)
    {
        KeyVaultSecret secret = _secretClient.GetSecret(secretName);
        return secret.Value;
    }
}