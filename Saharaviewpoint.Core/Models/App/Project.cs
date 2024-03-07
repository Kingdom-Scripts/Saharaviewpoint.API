using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saharaviewpoint.Core.Models.App;

public class Project : BaseAppModel
{
    [Required]
    [MaxLength(50)]
    public string Title { get; set; }

    [MaxLength(5000)]
    public string? Description { get; set; }

    public required string SizeOfSite { get; set; }

    [Column(TypeName = "decimal(19, 2)")]
    public decimal Budget { get; set; }

    public required string Location { get; set; }

    public int TypeId { get; set; }

    [MaxLength(500)]
    public string? SurroundingFacilities { get; set; }

    public int? DesignId { get; set; }

    public int? AssigneeId { get; set; }
    
    [Required]
    [MaxLength(15)]
    public required string Status { get; set; }
    
    public DateTime StartDate { get; set; }

    public DateTime DueDate { get; set; }
    
    [Required]
    public int Order { get; set; }
    
    [Required]
    public bool IsPriority { get; set; }
    
    [Required]
    public int CreatedById { get; set; }

    [Required]
    public bool IsDeleted { get; set; } = false;

    public int? DeletedById { get; set; }

    public DateTime? DateDeleted { get; set; }

    public ProjectType Type { get; set; }

    public User Assignee { get; set; }

    public Document Design { get; set; }
    
    public User CreatedBy { get; set; }

    public User DeletedBy { get; set; }
}
