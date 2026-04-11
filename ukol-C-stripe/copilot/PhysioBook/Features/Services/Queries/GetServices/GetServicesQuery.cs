namespace PhysioBook.Features.Services.Queries.GetServices;

public class GetServicesQuery : IQuery<GetServicesResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
