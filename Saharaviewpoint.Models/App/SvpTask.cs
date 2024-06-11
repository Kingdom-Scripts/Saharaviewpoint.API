using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class SvpTask : BaseAppModel
{
    [Required] public int ProjectId { get; set; }
    public int? ParentId { get; set; }
    [Required][MaxLength(15)] public string Type { get; set; } = null!;
    [Required][MaxLength(15)] public string Status { get; set; } = null!;
    [Required][MaxLength(255)] public string Summary { get; set; } = null!;
    [MaxLength(5000)] public string? Description { get; set; }
    public int CreatedById { get; set; }
    public DateTime ExpectedStartDate { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public int? UpdatedById { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Order { get; set; }

    [Required] public bool IsDeleted { get; set; } = false;
    public int? DeletedById { get; set; }
    public DateTime? DateDeleted { get; set; }

    public SvpTask? Parent { get; set; }
    public User? CreatedBy { get; set; }
    public User? UpdatedBy { get; set; }
    public Project? Project { get; set; }
    public User? DeletedBy { get; set; }
    public List<TaskAttachment> TaskAttachments { get; set; } = new();
}