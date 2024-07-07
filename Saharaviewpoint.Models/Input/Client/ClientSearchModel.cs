
namespace Saharaviewpoint.Models.Input.Client;

public class ClientSearchModel : PagingOptionModel
{
    public DateTime? DateJoinedStart { get; set; }
    public DateTime? DateJoinedEnd { get; set; }
    public bool IsActiveOnly { get; set; } = false;
    public bool IsInactiveOnly { get; set; } = false;
}