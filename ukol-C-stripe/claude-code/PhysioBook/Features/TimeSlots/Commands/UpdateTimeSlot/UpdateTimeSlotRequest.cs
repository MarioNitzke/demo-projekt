namespace PhysioBook.Features.TimeSlots.Commands.UpdateTimeSlot;

public record UpdateTimeSlotRequest(int DayOfWeek, string StartTime, string EndTime, bool IsAvailable);
