// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Saharaviewpoint.Models.Utilities;

/// <summary>
/// Represents a forbidden error result, derived from the <see cref="Result"/> class.
/// </summary>
public class ForbiddenResult : Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenResult"/> class with a default failure status.
    /// </summary>
    public ForbiddenResult() : base(false)
    {
        Status = StatusCodes.Status403Forbidden;
        Title = "Forbidden Request";
        Message = "You do not have permission to access this resource.";
    }

    /// <summary>
    /// Additional details about the error.
    /// </summary>
    [JsonIgnore]
    public new string Detail { get; set; }

    /// <summary>
    /// The instance where the error occurred.
    /// </summary>
    [JsonIgnore]
    public new string Instance { get; set; }

    /// <summary>
    /// The path associated with the error.
    /// </summary>
    [JsonIgnore]
    public new string Path { get; set; }

    [JsonIgnore]
    public new object Content { get; set; }

    [JsonIgnore]
    public new Paging Paging { get; set; }
}