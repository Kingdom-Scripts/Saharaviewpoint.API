using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App;

public partial class UserRole : BaseAppModel
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
    /// The date this role was created for this user
    /// </summary>
    [Required]
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// A foreign key to the user who assigned this role to this user
    /// </summary>
    [Required]
    public int CreatedById { get; set; }

    public Role Role { get; set; }
    public User User { get; set; }
}