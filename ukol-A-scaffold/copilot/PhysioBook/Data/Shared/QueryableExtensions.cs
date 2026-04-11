namespace PhysioBook.Data.Shared;

public static class QueryableExtensions
{
    public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize, CancellationToken ct)
    {
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedList<T>(items, pageNumber, pageSize, totalCount);
    }
}

