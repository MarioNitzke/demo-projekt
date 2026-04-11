namespace PhysioBook.Features.Bookings.Queries.GetAvailability;

public record GetAvailabilityResponse(
    string Date,
    Guid ServiceId,
    string ServiceName,
    List<AvailableSlotDto> AvailableSlots);

public record AvailableSlotDto(string StartTime, string EndTime);
