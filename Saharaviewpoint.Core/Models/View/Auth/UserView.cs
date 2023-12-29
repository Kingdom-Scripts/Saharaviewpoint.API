using System.Text.Json.Serialization;

namespace Saharaviewpoint.Core.Models.View.Auth;

public class UserView
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Uid { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Phone { get; set; }
}