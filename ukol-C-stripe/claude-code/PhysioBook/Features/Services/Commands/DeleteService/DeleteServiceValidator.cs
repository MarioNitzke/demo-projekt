namespace PhysioBook.Features.Services.Commands.DeleteService;

public class DeleteServiceValidator : AbstractValidator<DeleteServiceCommand>
{
    public DeleteServiceValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
