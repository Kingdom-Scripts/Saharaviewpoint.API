using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App;

public class BaseAppModel
{
    [Required]
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}