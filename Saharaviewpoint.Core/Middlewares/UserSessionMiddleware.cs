// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Saharaviewpoint.Models.Constants;
using Saharaviewpoint.Models.Input.Auth;

namespace Saharaviewpoint.Core.Middlewares;

public class UserSessionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, UserSession session)
    {
        if (context.User.Identities.Any(x => x.IsAuthenticated))
        {
            int.TryParse(context.User.Claims.SingleOrDefault(c => c.Type == "sid")?.Value, out int UserId);

            session.UserId = UserId;
            session.Uid = context.User.Claims.SingleOrDefault(c => c.Type == "uid")?.Value;
            session.Type = context.User.Claims.SingleOrDefault(c => c.Type == "type")?.Value;
            session.Name = context.User.Claims.SingleOrDefault(c => c.Type == "name")?.Value;
            session.Roles = context.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        }

        // get the app type
        string? token = context.Request.Headers["AppType"];
        if (token is not null)
        {
            session.AppType = token == "Client" ? AppTypes.Client : AppTypes.Admin;
        }

        // Call the next delegate/middleware in the pipeline
        await _next.Invoke(context);
    }
}