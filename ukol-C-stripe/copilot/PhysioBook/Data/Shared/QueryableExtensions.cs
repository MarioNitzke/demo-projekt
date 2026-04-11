namespace PhysioBook.Data.Shared;

public static class QueryableExtensions
{
    public static Task<PagedList<T>> ToPagedListAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        return PagedList<T>.CreateAsync(source, pageNumber, pageSize, cancellationToken);
    }
}
