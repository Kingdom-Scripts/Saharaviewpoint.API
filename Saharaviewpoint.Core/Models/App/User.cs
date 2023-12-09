namespace Saharaviewpoint.Core.Models.App
{
    public partial class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public UserTypeEnum Type { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastLoginDate { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
