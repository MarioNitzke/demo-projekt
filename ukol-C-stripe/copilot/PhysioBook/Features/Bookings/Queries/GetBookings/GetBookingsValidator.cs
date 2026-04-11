namespace PhysioBook.Features.Bookings.Queries.GetBookings;

public class GetBookingsValidator : AbstractValidator<GetBookingsQuery>
{
    public GetBookingsValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(50);
    }
}
