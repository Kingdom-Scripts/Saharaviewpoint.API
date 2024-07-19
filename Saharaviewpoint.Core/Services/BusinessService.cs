// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.Input.Auth;
using Saharaviewpoint.Models.Utilities;

namespace Saharaviewpoint.Core.Services;

public class BusinessService(UserSession userSession, SaharaviewpointContext context)
{
    private readonly SaharaviewpointContext _context = context;
    private readonly UserSession _userSession = userSession;

    public Task<Result> CreateBusiness()
    {
            throw new NotImplementedException();
        }
}