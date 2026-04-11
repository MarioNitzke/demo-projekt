namespace PhysioBook.Features.TimeSlots.Queries.GetTimeSlots;

public class GetTimeSlotsValidator : AbstractValidator<GetTimeSlotsQuery>
{
    public GetTimeSlotsValidator()
    {
        RuleFor(x => x.DayOfWeek)
            .InclusiveBetween(0, 6)
            .When(x => x.DayOfWeek.HasValue);
    }
}
