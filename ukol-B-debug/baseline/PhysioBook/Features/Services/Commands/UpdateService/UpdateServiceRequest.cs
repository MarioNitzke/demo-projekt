namespace PhysioBook.Features.Services.Commands.UpdateService;

public record UpdateServiceRequest(string Name, string Description, int DurationMinutes, decimal Price);
