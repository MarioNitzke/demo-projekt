using PhysioBook.Data.Entities;
using PhysioBook.Data.Shared;

namespace PhysioBook.Features.Services.Queries.GetServices;

public static class GetServicesMappings
{
    public static GetServicesQuery ToQuery(this GetServicesRequest request)
    {
        return new GetServicesQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }

    public static GetServicesResponse ToResponse(this PagedList<Service> pagedList)
    {
        return new GetServicesResponse(
            pagedList.Items.Select(s => s.ToItemResponse()).ToList(),
            pagedList.TotalCount,
            pagedList.PageNumber,
            pagedList.PageSize,
            pagedList.TotalPages);
    }

    public static GetServicesItemResponse ToItemResponse(this Service service)
    {
        return new GetServicesItemResponse(
            service.Id,
            service.Name,
            service.Description,
            service.DurationMinutes,
            service.Price,
            service.IsActive);
    }
}
