// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;
public class VideoAsset
{
    [Key]
    public int DocumentId { get; set; }
    public string Iframe { get; set; }
    public string Player { get; set; }
    public string Hls { get; set; }
    public string Thumbnail { get; set; }
    public string Mp4 { get; set; }

    public Document Document { get; set; }
}
