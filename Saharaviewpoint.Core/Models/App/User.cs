using Saharaviewpoint.Core.Models.App.Enums;
using System.ComponentModel.DataAnnotations;

namespace Saharaviewpoint.Core.Models.App
{
    public partial class User
    {
        public int Id { get; set; }

        public Guid Uid { get; set; } = Guid.NewGuid();

        [MaxLength(50)]
        public string Email { get; internal set; }

        [MaxLength(20)]
        public string Username { get; set; }

        public UserTypes Type { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [MaxLength(50)]
        public string LastName { get; set; }

        [MaxLength(25)]
        public string Phone { get; set; }

        [MaxLength(255)]
        public string HashedPassword { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime LastLoginDate { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
