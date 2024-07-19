// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.App.Constants;

public class ProjectLogTypes
{
    public static readonly ProjectLogTypes Create = new("Create");
    public static readonly ProjectLogTypes Update = new("Update");
    public static readonly ProjectLogTypes Delete = new("Delete");
    public static readonly ProjectLogTypes Assignment = new("Assignment");
    public static readonly ProjectLogTypes StatusChange = new("StatusChange");
    public static readonly ProjectLogTypes Approvals = new("Approvals");

    public string Value { get; }

    private ProjectLogTypes(string value)
    {
        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}