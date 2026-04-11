namespace PhysioBook.Features.Bookings.Queries.GetAvailability;

public class GetAvailabilityValidator : AbstractValidator<GetAvailabilityQuery>
{
    public GetAvailabilityValidator()
    {
        RuleFor(x => x.ServiceId)
            .NotEmpty();

        RuleFor(x => x.Date)
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("Date must not be in the past");
    }
}
