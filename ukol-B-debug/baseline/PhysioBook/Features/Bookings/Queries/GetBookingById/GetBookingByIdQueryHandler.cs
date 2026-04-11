using PhysioBook.Data;

namespace PhysioBook.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdQueryHandler : IQueryHandler<GetBookingByIdQuery, GetBookingByIdResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetBookingByIdQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetBookingByIdResponse> Handle(GetBookingByIdQuery query, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var booking = await context.Bookings
            .Include(b => b.Service)
            .FirstOrDefaultAsync(b => b.Id == query.Id, ct)
            ?? throw new KeyNotFoundException($"Booking with Id '{query.Id}' was not found.");

        if (booking.ClientId != query.UserId && !query.IsAdmin)
            throw new UnauthorizedAccessException("You can only view your own bookings");

        return booking.ToResponse();
    }
}
