// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using FluentValidation;

namespace Saharaviewpoint.Models.Input.Task;

public class Assets
{
    public string iframe { get; set; }
    public string player { get; set; }
    public string hls { get; set; }
    public string thumbnail { get; set; }
    public string mp4 { get; set; }
}

public class VideoDetailModel
{
    public string videoId { get; set; }
    public string title { get; set; }
    public string description { get; set; }
    public bool @public { get; set; }
    public bool panoramic { get; set; }
    public bool mp4Support { get; set; }
    public DateTime publishedAt { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
    public List<object> tags { get; set; }
    public List<object> metadata { get; set; }
    public Source source { get; set; }
    public Assets assets { get; set; }
}

public class Source
{
    public string type { get; set; }
    public string uri { get; set; }
}

public class ApiVideoDetail : VideoDetailModel
{
    public string playerId { get; set; }
    public bool mp4Support { get; set; } = true;
    public bool @public { get; set; } = true;
    public int Duration { get; set; }
    public string ThumbnailUrl { get; set; }
}

public class VideoDetailValidator : AbstractValidator<VideoDetailModel>
{
    public VideoDetailValidator()
    {
        RuleFor(x => x.videoId).NotEmpty();
        RuleFor(x => x.title).NotEmpty();
    }
}
