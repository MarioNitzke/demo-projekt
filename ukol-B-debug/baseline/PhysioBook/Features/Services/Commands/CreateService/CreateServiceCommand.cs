namespace PhysioBook.Features.Services.Commands.CreateService;

public class CreateServiceCommand : IQuery<CreateServiceResponse>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
}
