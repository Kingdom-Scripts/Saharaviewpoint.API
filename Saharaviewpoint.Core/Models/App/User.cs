using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App;

public partial class User : BaseAppModel
{
    public Guid Uid { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(50)]
    public string Email { get; internal set; }

    [Required]
    [MaxLength(20)]
    public string Username { get; set; }

    [Required]
    public string Type { get; set; }

    [MaxLength(50)]
    public string FirstName { get; set; }

    [MaxLength(50)]
    public string LastName { get; set; }

    [MaxLength(25)]
    public string Phone { get; set; }

    [Required]
    [MaxLength(255)]
    public string HashedPassword { get; set; }

    [Required]
    public bool IsActive { get; set; } = true;

    [Required]
    public DateTime LastLoginDate { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; }
}