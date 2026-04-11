using PhysioBook.Data;
using PhysioBook.Data.Enums;

namespace PhysioBook.Features.Bookings.Queries.GetAvailability;

public class GetAvailabilityQueryHandler : IQueryHandler<GetAvailabilityQuery, GetAvailabilityResponse>
{
    private readonly IDbContextFactory<PhysioBookContext> _contextFactory;

    public GetAvailabilityQueryHandler(IDbContextFactory<PhysioBookContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<GetAvailabilityResponse> Handle(GetAvailabilityQuery query, CancellationToken ct)
    {
        using var context = await _contextFactory.CreateDbContextAsync(ct);

        var service = await context.Services.FindAsync(new object[] { query.ServiceId }, ct)
            ?? throw new KeyNotFoundException("Service not found");

        var dayOfWeek = query.Date.DayOfWeek;
        var timeSlots = await context.TimeSlots
            .Where(ts => ts.DayOfWeek == dayOfWeek && ts.IsAvailable)
            .OrderBy(ts => ts.StartTime)
            .ToListAsync(ct);

        var dateAsDateTime = query.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var existingBookings = await context.Bookings
            .Where(b => b.StartTime >= dateAsDateTime
                && b.StartTime < dateAsDateTime.AddDays(1)
                && b.Status != BookingStatus.Cancelled)
            .ToListAsync(ct);

        var availableSlots = new List<AvailableSlotDto>();
        var step = TimeSpan.FromMinutes(30); // Fixed 30-min step

        foreach (var slot in timeSlots)
        {
            var current = slot.StartTime;
            var slotEnd = slot.EndTime;

            while (true)
            {
                var candidateEndTimeOnly = current.Add(TimeSpan.FromMinutes(service.DurationMinutes));
                if (candidateEndTimeOnly > slotEnd)
                    break;

                var candidateStart = dateAsDateTime.Add(current.ToTimeSpan());
                var candidateEnd = dateAsDateTime.Add(candidateEndTimeOnly.ToTimeSpan());

                var isOverlapping = existingBookings.Any(b =>
                    b.StartTime < candidateEnd && b.EndTime > candidateEnd);

                if (!isOverlapping)
                {
                    availableSlots.Add(new AvailableSlotDto(
                        current.ToString("HH:mm"),
                        candidateEndTimeOnly.ToString("HH:mm")));
                }

                current = current.Add(step);
            }
        }

        return new GetAvailabilityResponse(
            query.Date.ToString("yyyy-MM-dd"),
            query.ServiceId,
            service.Name,
            availableSlots);
    }
}
