namespace Saharaviewpoint.Models.Input;

public class PagingOptionModel
{
    /// <summary>
    /// The page index/number to query. Defaults to 1
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// The page size to return or how many items to return. Defaults to 15
    /// </summary>
    public int PageSize { get; set; } = 15;

    /// <summary>
    /// The search query to filter the results
    /// </summary>
    public string? SearchQuery { get; set; }
}