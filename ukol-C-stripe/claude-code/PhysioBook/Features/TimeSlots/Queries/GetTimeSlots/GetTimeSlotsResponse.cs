namespace PhysioBook.Features.TimeSlots.Queries.GetTimeSlots;

public record GetTimeSlotsResponse(
    List<GetTimeSlotsItemResponse> Items);

public record GetTimeSlotsItemResponse(
    Guid Id,
    int DayOfWeek,
    string StartTime,
    string EndTime,
    bool IsAvailable);
