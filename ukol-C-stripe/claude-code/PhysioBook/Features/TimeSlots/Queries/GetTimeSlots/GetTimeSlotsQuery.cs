namespace PhysioBook.Features.TimeSlots.Queries.GetTimeSlots;

public class GetTimeSlotsQuery : IQuery<GetTimeSlotsResponse>
{
    public int? DayOfWeek { get; set; }
}
