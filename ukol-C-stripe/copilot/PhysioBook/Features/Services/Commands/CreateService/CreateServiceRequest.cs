namespace PhysioBook.Features.Services.Commands.CreateService;

public record CreateServiceRequest(string Name, string Description, int DurationMinutes, decimal Price);
