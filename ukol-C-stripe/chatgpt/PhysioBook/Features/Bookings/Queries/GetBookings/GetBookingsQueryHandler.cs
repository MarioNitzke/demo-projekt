using PhysioBook.Data;
using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Bookings.Queries.GetBookings;

public class GetBookingsQueryHandler : IQueryHandler<GetBookingsQuery, GetBookingsResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetBookingsQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetBookingsResponse> Handle(GetBookingsQuery query, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var bookingsQuery = context.Bookings
            .Include(b => b.Service)
            .AsQueryable();

        if (!query.IsAdmin)
        {
            bookingsQuery = bookingsQuery.Where(b => b.ClientId == query.UserId);
        }

        var orderedBookings = bookingsQuery.OrderByDescending(b => b.StartTime);

        var pagedList = await orderedBookings.ToPagedListAsync(query.PageNumber, query.PageSize, ct);

        return pagedList.ToResponse();
    }
}
