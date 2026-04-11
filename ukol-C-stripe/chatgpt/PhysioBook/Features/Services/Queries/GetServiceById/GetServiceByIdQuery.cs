namespace PhysioBook.Features.Services.Queries.GetServiceById;

public class GetServiceByIdQuery : IQuery<GetServiceByIdResponse>
{
    public Guid Id { get; set; }
}
