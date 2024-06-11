using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Models.App;

public class Code : BaseAppModel
{
    [MaxLength(255)]
    public string? Email { get; set; }

    [Required]
    [MaxLength(500)]
    public required string Token { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Purpose { get; set; }

    [Required]
    public required DateTime ExpiryDate { get; set; }

    public bool Used { get; set; } = false;

    [Required]
    public int OwnerId { get; set; }

    public User Owner { get; set; } = null!;
}