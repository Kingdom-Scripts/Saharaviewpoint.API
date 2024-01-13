using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App;

public class Document : BaseAppModel
{
    [Required]
    public string Name { get; set; }

    [StringLength(10)]
    [Required]
    public string Extension { get; set; }

    [StringLength(25)]
    [Required]
    public string Type { get; set; }

    [MaxLength(50)]
    [Required]
    public string Folder { get; set; }

    [Required]
    public int CreatedById { get; set; }

    public User CreatedBy { get; set; }
}
