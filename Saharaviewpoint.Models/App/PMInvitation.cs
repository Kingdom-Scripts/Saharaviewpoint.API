// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class PMInvitation : BaseAppModel
{
    [Required]
    [MaxLength(50)]
    public required string Email { get; set; }

    [MaxLength(50)]
    public required string FirstName { get; set; }

    [MaxLength(50)]
    public required string LastName { get; set; }

    [MaxLength(25)] public string Phone { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }

    public bool IsFulfilled { get; set; } = false;

    public bool IsExpired { get; set; } = false;

    public bool EmailSent { get; set; } = false;

    // TODO: make this required`
    public int? CreatedById { get; set; }

    public User? CreatedBy { get; set; }
}