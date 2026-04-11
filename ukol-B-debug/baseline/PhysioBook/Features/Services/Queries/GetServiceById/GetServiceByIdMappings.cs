using PhysioBook.Data.Entities;

namespace PhysioBook.Features.Services.Queries.GetServiceById;

public static class GetServiceByIdMappings
{
    public static GetServiceByIdQuery ToQuery(this GetServiceByIdRequest request)
    {
        return new GetServiceByIdQuery
        {
            Id = request.Id
        };
    }

    public static GetServiceByIdResponse ToResponse(this Service service)
    {
        return new GetServiceByIdResponse(
            service.Id,
            service.Name,
            service.Description,
            service.DurationMinutes,
            service.Price,
            service.IsActive);
    }
}
