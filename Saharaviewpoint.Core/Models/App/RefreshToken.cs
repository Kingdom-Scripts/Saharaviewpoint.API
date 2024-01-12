using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App;

public class RefreshToken
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    public User User { get; set; }

    [Required]
    [MaxLength(255)]
    public string Code { get; set; }

    public DateTime ExpiresAt { get; set; }
}