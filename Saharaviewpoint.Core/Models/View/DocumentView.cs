using Saharaviewpoint.Core.Models.App.Constants;
using System.Text.Json.Serialization;

namespace Saharaviewpoint.Core.Models.View;

public class DocumentView
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    [JsonIgnore]
    public string Folder { get; set; }

    public string Extension { get; set; }

    public string Url
    {
        get
        {
            return $"{Folder}/{Name}.{Extension}";
        }
    }

    public string ThumbnailUrl
    {
        get
        {
            return Type == DocumentTypes.IMAGE
                ? $"{Folder}/thumbnails/{Name}.{Extension}"
                : $"assets/thumbnails/{Type}.png";
        }
    }
}
