// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App
{
    public class TaskComment : BaseAppModel
    {
        [Required]
        public int TaskId { get; set; }

        public int? ParentId { get; set; }

        [Required]
        [MaxLength]
        public string Message { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string FullName { get; set; } = null!;

        [Required]
        public int CreatedById { get; set; }

        public TaskComment? Parent { get; set; }
        public SvpTask? Task { get; set; }
        public User? CreatedBy { get; set; }
        public ICollection<TaskComment> Children { get; set; } = new List<TaskComment>();
    }
}