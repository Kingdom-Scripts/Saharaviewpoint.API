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
    [StringLength(255)]
    public string Iframe { get; set; }
    [StringLength(50)]
    public string Player { get; set; }
    [StringLength(255)]
    public string Hls { get; set; }
    [StringLength(255)]
    public string Thumbnail { get; set; }
    [StringLength(255)]
    public string Mp4 { get; set; }

    public Document Document { get; set; }
}