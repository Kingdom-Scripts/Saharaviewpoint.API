using Saharaviewpoint.Core.Models.App.Enums;

namespace Saharaviewpoint.Core.Models.View.Auth
{
    public class UserProfileView
    {
        public Guid Uid { get; set; } = new Guid();
        public string Email { get; internal set; }
        public string Username { get; set; }
        public UserTypes Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public DateTime LastLoginDate { get; set; }
    }
}
