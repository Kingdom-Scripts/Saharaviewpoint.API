using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App
{
    public class Code : BaseAppModel
    {
        [MaxLength(500)]
        public required string Token { get; set; }

        public required string Purpose { get; set; }

        public required DateTime ExpiryDate { get; set; }

        public bool Used { get; set; } = false;
    }
}
