// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class UserRole : BaseAppModel
{
    /// <summary>
    /// A foreign key to the user this role is attached to
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// A foreign key to the role this user possess
    /// </summary>
    [Required]
    public int RoleId { get; set; }

    /// <summary>
    /// A foreign key to the user who assigned this role to this user
    /// </summary>
    [Required]
    public int CreatedById { get; set; }
    public Role? Role { get; set; }
    public User? User { get; set; }
}