namespace PhysioBook.Features.Services.Commands.CreateService;

public record CreateServiceResponse(Guid Id, string Name, string Description, int DurationMinutes, decimal Price, bool IsActive);
