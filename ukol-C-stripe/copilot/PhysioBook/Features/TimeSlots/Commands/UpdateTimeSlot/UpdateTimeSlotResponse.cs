namespace PhysioBook.Features.TimeSlots.Commands.UpdateTimeSlot;

public record UpdateTimeSlotResponse(Guid Id, int DayOfWeek, string StartTime, string EndTime, bool IsAvailable);
