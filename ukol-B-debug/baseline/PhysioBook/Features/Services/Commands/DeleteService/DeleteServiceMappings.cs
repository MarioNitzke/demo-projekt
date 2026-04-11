namespace PhysioBook.Features.Services.Commands.DeleteService;

public static class DeleteServiceMappings
{
    public static DeleteServiceCommand ToCommand(this DeleteServiceRequest request)
    {
        return new DeleteServiceCommand
        {
            Id = request.Id
        };
    }
}
