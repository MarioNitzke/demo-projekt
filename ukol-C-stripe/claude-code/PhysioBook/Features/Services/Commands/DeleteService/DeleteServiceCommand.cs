namespace PhysioBook.Features.Services.Commands.DeleteService;

public class DeleteServiceCommand : IQuery<DeleteServiceResponse>
{
    public Guid Id { get; set; }
}
