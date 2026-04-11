namespace PhysioBook.Features.Services.Queries.GetServices;

public record GetServicesResponse(
    List<GetServicesItemResponse> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages);

public record GetServicesItemResponse(
    Guid Id,
    string Name,
    string Description,
    int DurationMinutes,
    decimal Price,
    bool IsActive);
