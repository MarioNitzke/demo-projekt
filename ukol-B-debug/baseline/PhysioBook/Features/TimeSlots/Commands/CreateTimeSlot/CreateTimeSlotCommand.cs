namespace PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;

public class CreateTimeSlotCommand : IQuery<CreateTimeSlotResponse>
{
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; }
}
