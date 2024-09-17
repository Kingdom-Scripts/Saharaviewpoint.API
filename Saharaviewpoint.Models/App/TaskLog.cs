// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App
{
    public class TaskLog : BaseAppModel
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; } = null!;

        [StringLength(1000)]
        public string PreviousState { get; set; }

        [StringLength(1000)]
        public string CurrentState { get; set; }

        [Required]
        public int CreatedById { get; set; }

        public SvpTask Task { get; set; }
        public User CreatedBy { get; set; }

        [StringLength(5000)]
        public string Remark { get; set; }
    }
}