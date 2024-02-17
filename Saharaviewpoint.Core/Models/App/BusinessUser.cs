using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App;

public class BusinessUser : BaseAppModel
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int BusinessId { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
    public Business Business { get; set; }
}