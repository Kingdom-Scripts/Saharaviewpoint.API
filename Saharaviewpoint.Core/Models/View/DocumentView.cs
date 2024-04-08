using Saharaviewpoint.Core.Models.App.Constants;
using System.Text.Json.Serialization;

namespace Saharaviewpoint.Core.Models.View;

public class DocumentView
{
    [JsonIgnore]
    public int Id { get; set; }

    public required string Name { get; set; }

    public required string Type { get; set; }

    public required string Url { get; set; }

    public required string ThumbnailUrl { get; set; }
}