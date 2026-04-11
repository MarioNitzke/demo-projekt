namespace PhysioBook.Features.Services.Commands.UpdateService;

public class UpdateServiceCommand : IQuery<UpdateServiceResponse>
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
}
