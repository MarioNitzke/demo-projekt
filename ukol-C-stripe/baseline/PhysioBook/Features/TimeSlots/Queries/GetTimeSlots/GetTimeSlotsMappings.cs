using PhysioBook.Data.Entities;

namespace PhysioBook.Features.TimeSlots.Queries.GetTimeSlots;

public static class GetTimeSlotsMappings
{
    public static GetTimeSlotsQuery ToQuery(this GetTimeSlotsRequest request)
    {
        return new GetTimeSlotsQuery
        {
            DayOfWeek = request.DayOfWeek
        };
    }

    public static GetTimeSlotsResponse ToResponse(this List<TimeSlot> timeSlots)
    {
        return new GetTimeSlotsResponse(
            timeSlots.Select(t => t.ToItemResponse()).ToList());
    }

    public static GetTimeSlotsItemResponse ToItemResponse(this TimeSlot entity)
    {
        return new GetTimeSlotsItemResponse(
            entity.Id,
            (int)entity.DayOfWeek,
            entity.StartTime.ToString("HH:mm"),
            entity.EndTime.ToString("HH:mm"),
            entity.IsAvailable);
    }
}
