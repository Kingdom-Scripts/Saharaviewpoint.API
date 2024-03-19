using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App
{
    public class PMInvitation : BaseAppModel
    {
        [Required]
        [MaxLength(50)]
        public required string Email { get; set; }

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MaxLength(25)]
        public string? Phone { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsFulfilled { get; set; } = false;

        public bool IsExpired { get; set; } = false;
    }
}
