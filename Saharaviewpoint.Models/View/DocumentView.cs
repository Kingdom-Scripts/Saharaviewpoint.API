using Saharaviewpoint.Models.App.Constants;
using System.Text.Json.Serialization;

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