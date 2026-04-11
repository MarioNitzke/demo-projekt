namespace PhysioBook.Features.Bookings.Queries.GetBookingById;

public class GetBookingByIdValidator : AbstractValidator<GetBookingByIdQuery>
{
    public GetBookingByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
