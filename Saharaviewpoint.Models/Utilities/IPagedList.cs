namespace Saharaviewpoint.Models.Utilities;

public interface IPagedList
{
    int PageIndex { get; }
    int PageSize { get; }
    int TotalPages { get; }
    int TotalItems { get; }
    bool HasPreviousPage { get; }
    bool HasNextPage { get; }
}