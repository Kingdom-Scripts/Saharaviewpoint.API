namespace Saharaviewpoint.Core.Models.App
{
    public class EpicTask
    {
        public required int Id { get; set; }
        public required int TaskId { get; set; }
        public required int CreatedById { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public SvpTask? Task { get; set; }
        public User? CreatedBy { get; set; }
    }
}