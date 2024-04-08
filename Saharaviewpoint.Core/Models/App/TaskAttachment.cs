namespace Saharaviewpoint.Core.Models.App
{
    public class TaskAttachment : BaseAppModel
    {
        public int TaskId { get; set; }
        public int DocumentId { get; set; }

        public SvpTask? Task { get; set; }
        public Document? Document { get; set; }
    }
}