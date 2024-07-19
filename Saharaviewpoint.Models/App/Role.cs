// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class Role
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public ICollection<UserRole> UserRoles { get; set; }
}