// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

namespace Saharaviewpoint.Models.View;

public class DocumentView
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Type { get; set; }

    public required string Url { get; set; }

    public required string ThumbnailUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}