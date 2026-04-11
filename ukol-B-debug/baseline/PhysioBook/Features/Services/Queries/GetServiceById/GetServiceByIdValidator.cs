namespace PhysioBook.Features.Services.Queries.GetServiceById;

public class GetServiceByIdValidator : AbstractValidator<GetServiceByIdQuery>
{
    public GetServiceByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
