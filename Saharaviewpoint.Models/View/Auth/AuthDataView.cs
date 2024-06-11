namespace Saharaviewpoint.Models.View.Auth;

public class AuthDataView
{
    public UserView User { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}