namespace Saharaviewpoint.Core.Models.App.Constants;

public enum Roles
{
    SvpAdmin = 1,
    SvpManager = 2,
    BusinessAdmin = 3,
    BusinessManager = 4,
    BusinessClient = 5,
    Client = 6,
    SuperAdmin = 7
}

public static class RolesConstants
{
    public static readonly string SvpAdmin = "SvpAdmin";
    public static readonly string SvpManager = "SvpManager";
    public static readonly string BusinessAdmin = "BusinessAdmin";
    public static readonly string BusinessManager = "BusinessManager";
    public static readonly string BusinessClient = "BusinessClient";
    public static readonly string Client = "Client";
    public static readonly string SuperAdmin = "SuperAdmin";
}