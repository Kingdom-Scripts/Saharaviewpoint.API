using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saharaviewpoint.Models.App;

public class Project : BaseAppModel
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [MaxLength(5000)]
    public string? Description { get; set; }

    [Required] public string SizeOfSite { get; set; } = null!;

    [Column(TypeName = "decimal(19, 2)")]
    public decimal Budget { get; set; }

    public string Location { get; set; } = null!;

    public int TypeId { get; set; }

    [MaxLength(500)]
    public string? SurroundingFacilities { get; set; }

    public int? DesignId { get; set; }

    public int? AssigneeId { get; set; }

    [Required]
    [MaxLength(15)]
    public string Status { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }

    [Required]
    public int Order { get; set; }

    [Required]
    public bool IsPriority { get; set; }

    [Required]
    public int CreatedById { get; set; }

    public int? UpdatedById { get; set; }
    public DateTime? UpdatedOn { get; set; }

    [Required]
    public bool IsDeleted { get; set; } = false;

    public int? DeletedById { get; set; }

    public DateTime? DateDeleted { get; set; }

    public ProjectType Type { get; set; }

    // TODO, make this required
    [MaxLength(255)]
    public List<string> FolderNames { get; set; } = new();

    public User? Assignee { get; set; }
    public Document? Design { get; set; }

    public User? CreatedBy { get; set; }
    public User? UpdatedBy { get; set; }

    public User? DeletedBy { get; set; }
}