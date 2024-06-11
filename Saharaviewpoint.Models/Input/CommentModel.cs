namespace Saharaviewpoint.Models.Input
{
    public class CommentModel
    {
        public int? ParentId { get; set; }
        public string Message { get; set; } = null!;
    }
}
