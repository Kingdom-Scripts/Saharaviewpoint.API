namespace Saharaviewpoint.Core.Models.Input.Auth;

public class UserSession
{
    public int UserId { get; set; }
    public string Uid { get; set; }
    public string Type { get; set; }
    public string BusinessCode { get; set; }
}