using PhysioBook.Data;

namespace PhysioBook.Features.TimeSlots.Queries.GetTimeSlots;

public class GetTimeSlotsQueryHandler : IQueryHandler<GetTimeSlotsQuery, GetTimeSlotsResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetTimeSlotsQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetTimeSlotsResponse> Handle(GetTimeSlotsQuery query, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var timeSlotsQuery = context.TimeSlots.AsQueryable();

        if (query.DayOfWeek.HasValue)
        {
            var dayOfWeek = (DayOfWeek)query.DayOfWeek.Value;
            timeSlotsQuery = timeSlotsQuery.Where(t => t.DayOfWeek == dayOfWeek);
        }

        var timeSlots = await timeSlotsQuery
            .OrderBy(t => t.DayOfWeek)
            .ThenBy(t => t.StartTime)
            .ToListAsync(ct);

        return timeSlots.ToResponse();
    }
}
