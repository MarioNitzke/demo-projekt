namespace PhysioBook.Features.Bookings.Queries.GetAvailability;

public record GetAvailabilityRequest(Guid ServiceId, DateOnly Date);
