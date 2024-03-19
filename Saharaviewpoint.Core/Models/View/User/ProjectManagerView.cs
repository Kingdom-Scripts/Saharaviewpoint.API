using System.Text.Json.Serialization;

namespace Saharaviewpoint.Core.Models.View.User
{
    public class ProjectManagerView
    {
        [JsonIgnore]
        public int Id { get; set; }

        public required Guid Uid { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public required string Email { get; set; }
        public int NoOfProjects { get; set; } = 0;
        public bool IsActive { get; set; }
    }
}
