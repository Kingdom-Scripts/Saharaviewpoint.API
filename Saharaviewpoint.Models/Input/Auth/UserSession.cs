using Saharaviewpoint.Models.App.Constants;

namespace Saharaviewpoint.Models.Input.Auth;

public class UserSession
{
    public int UserId { get; set; }
    public string Uid { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? BusinessCode { get; set; }

    private List<string> _roles = new();

    public List<string> Roles
    {
        set => _roles = value;
    }

    public bool InRole(params string[] roles)
    {
        return roles.Any(role => _roles.Contains(role));
    }

    public bool IsAnyAdmin => InRole(RolesConstants.SuperAdmin, RolesConstants.SvpAdmin);
    public bool IsClient => InRole(RolesConstants.Client);
    public bool IsSuperAdmin => InRole(RolesConstants.SuperAdmin);
    public bool IsSvpAdmin => InRole(RolesConstants.SvpAdmin);
}