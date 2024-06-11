namespace Saharaviewpoint.Models.App;

public class Login : BaseAppModel
{
    public int UserId { get; set; }
    public required string HashedToken { get; set; }
    public required string Domain { get; set; }
    public required DateTime ExpiresAt { get; set; }

    public User? User { get; set; }
}
