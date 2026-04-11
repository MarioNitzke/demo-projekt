namespace PhysioBook.Features.Services.Queries.GetServiceById;

public record GetServiceByIdResponse(
    Guid Id,
    string Name,
    string Description,
    int DurationMinutes,
    decimal Price,
    bool IsActive);
