using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App
{
    public class SvpTask : BaseAppModel
    {
        public required int ProjectId { get; set; }
        public int? ParentId { get; set; }
        [MaxLength(15)] public required string Type { get; set; }
        [MaxLength(15)] public required string Status { get; set; }
        [MaxLength(255)] public required string Summary { get; set; }
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
        public List<TaskAttachment> Attachments { get; set; } = new();
    }
}