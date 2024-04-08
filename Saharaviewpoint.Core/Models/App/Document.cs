using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App;

public class Document : BaseAppModel
{
    [Required]
    public string Name { get; set; }

    [StringLength(25)]
    [Required]
    public string Type { get; set; }

    [MaxLength(255)]
    public required string Url { get; set; }

    [MaxLength(255)]
    public required string ThumbnailUrl { get; set; }

    [Required]
    public int CreatedById { get; set; }

    public User? CreatedBy { get; set; }
}