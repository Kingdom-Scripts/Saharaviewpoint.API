namespace Saharaviewpoint.Core.Models.View.Auth;

public class AuthDataView
{
    public UserView User { get; internal set; }
    public string Token { get; internal set; }
    public string RefreshToken { get; internal set; }
    public DateTime ExpiresAt { get; internal set; }
}