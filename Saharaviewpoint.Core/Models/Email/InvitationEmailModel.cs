namespace Saharaviewpoint.Core.Models.Email;

public class InvitationEmailModel
{
    public required string Token { get; set; }
    public required string UserType { get; set; }
    public required string RecipientName { get; set; }
    public required string RecipientEmail { get; set; }
    public required string SenderName { get; set; }
}