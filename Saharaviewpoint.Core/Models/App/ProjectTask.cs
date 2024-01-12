namespace Saharaviewpoint.Core.Models.App;

public class ProjectTask : BaseAppModel
{
    public int ProjectId { get; set; }
    
    public string Title { get; set; }

    public string Description { get; set; }

    public string Status { get; set; }

    public short Order { get; set; }


}