namespace PhysioBook.Features.Services.Queries.GetServices;

public class GetServicesValidator : AbstractValidator<GetServicesQuery>
{
    public GetServicesValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(50);
    }
}
