namespace Saharaviewpoint.Core.Models.App
{
    public class UserDetail
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string Type { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }
    }
}