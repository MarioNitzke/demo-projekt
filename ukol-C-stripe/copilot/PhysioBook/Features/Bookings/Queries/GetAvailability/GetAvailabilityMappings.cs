namespace PhysioBook.Features.Bookings.Queries.GetAvailability;

public static class GetAvailabilityMappings
{
    public static GetAvailabilityQuery ToQuery(this GetAvailabilityRequest request)
    {
        return new GetAvailabilityQuery
        {
            ServiceId = request.ServiceId,
            Date = request.Date
        };
    }
}
