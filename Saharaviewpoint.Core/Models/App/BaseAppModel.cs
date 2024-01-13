using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App;

public class BaseAppModel
{
    [Required]
    public int Id { get; set; }

    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
}
