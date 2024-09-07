// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Saharaviewpoint.Core.Utilities;

/// <summary>
///     Utility class for HTTP-related operations.
/// </summary>
public static class HttpUtilities
{
    /// <summary>
    ///     Converts an object to JSON content for HTTP requests.
    /// </summary>
    /// <param name="obj">The object to be serialized to JSON content.</param>
    /// <returns>An instance of <see cref="StringContent" /> representing JSON content.</returns>
    public static StringContent ToJsonContent(this object obj)
    {
        StringContent jsonContent =
            new(JsonSerializer.Serialize(obj, new JsonSerializerOptions(JsonSerializerDefaults.Web)), Encoding.UTF8,
                MediaTypeNames.Application.Json);

        return jsonContent;
    }
}