// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Interfaces;

public interface ITokenHandler
{
    Task<Result> GenerateJwtToken(User user);

    Task<Result> RefreshJwtToken(string refreshToken);

    Task InvalidateToken(string userReference);

    Task<bool> ValidateToken(string uid, string token, string domain);
}