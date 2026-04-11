namespace PhysioBook.Features.Services.Commands.UpdateService;

public record UpdateServiceResponse(Guid Id, string Name, string Description, int DurationMinutes, decimal Price, bool IsActive);
