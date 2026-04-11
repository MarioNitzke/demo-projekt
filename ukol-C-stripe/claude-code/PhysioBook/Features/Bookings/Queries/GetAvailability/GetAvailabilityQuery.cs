namespace PhysioBook.Features.Bookings.Queries.GetAvailability;

public class GetAvailabilityQuery : IQuery<GetAvailabilityResponse>
{
    public Guid ServiceId { get; set; }
    public DateOnly Date { get; set; }
}
