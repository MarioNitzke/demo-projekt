namespace PhysioBook.Features.TimeSlots.Commands.CreateTimeSlot;

public record CreateTimeSlotRequest(int DayOfWeek, string StartTime, string EndTime, bool IsAvailable);
