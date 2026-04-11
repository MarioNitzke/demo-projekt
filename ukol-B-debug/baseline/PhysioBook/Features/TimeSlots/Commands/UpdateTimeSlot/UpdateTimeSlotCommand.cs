namespace PhysioBook.Features.TimeSlots.Commands.UpdateTimeSlot;

public class UpdateTimeSlotCommand : IQuery<UpdateTimeSlotResponse>
{
    public Guid Id { get; set; }
    public int DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; }
}
