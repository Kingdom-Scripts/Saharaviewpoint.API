// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Constants;

namespace Saharaviewpoint.Models.Input.Auth;

public class UserSession
{
    public int UserId { get; set; }
    public string Uid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string BusinessCode { get; set; }

    private List<string> _roles = [];

    public List<string> Roles
    {
        set => _roles = value;
    }

    public bool InRole(params string[] roles)
    {
        return roles.Any(role => _roles.Contains(role));
    }

    public bool IsClient => InRole(RolesConstants.Client);
    public bool IsBusinessAdmin => InRole(RolesConstants.BusinessAdmin);
    public bool IsBusinessClient => InRole(RolesConstants.BusinessClient);
    public bool IsAnySvpAdmin => InRole(RolesConstants.SuperAdmin, RolesConstants.SvpAdmin);
    public bool IsSuperAdmin => InRole(RolesConstants.SuperAdmin);
    public bool IsSvpAdmin => InRole(RolesConstants.SvpAdmin);
    public bool IsProjectManager => InRole(RolesConstants.SvpManager);

    public AppTypes? AppType { get; set; }

    public bool FilterByClient => AppType == AppTypes.Client && IsClient;
    public bool FilterByBusinessAdmin => AppType == AppTypes.Client && IsBusinessAdmin;
    public bool FilterByBusinessClient => AppType == AppTypes.Client && IsBusinessClient;
    public bool FilterByAnyClient => AppType == AppTypes.Client && (IsClient || IsBusinessClient);

    public bool FilterBySvpManager => AppType == AppTypes.Admin && IsProjectManager;
}