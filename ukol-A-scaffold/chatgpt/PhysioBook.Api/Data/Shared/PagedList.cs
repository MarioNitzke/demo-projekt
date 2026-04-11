namespace PhysioBook.Api.Data.Shared;

public sealed class PagedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int PageNumber { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }

    public PagedList(IReadOnlyCollection<T> items, int pageNumber, int pageSize, int totalCount)
    {
        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);
    }
}
