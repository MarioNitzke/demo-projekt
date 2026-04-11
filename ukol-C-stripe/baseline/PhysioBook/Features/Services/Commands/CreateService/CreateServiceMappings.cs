using PhysioBook.Data.Entities;

namespace PhysioBook.Features.Services.Commands.CreateService;

public static class CreateServiceMappings
{
    public static CreateServiceCommand ToCommand(this CreateServiceRequest request)
    {
        return new CreateServiceCommand
        {
            Name = request.Name,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price
        };
    }

    public static Service ToEntity(this CreateServiceCommand command)
    {
        return new Service
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            DurationMinutes = command.DurationMinutes,
            Price = command.Price,
            IsActive = true
        };
    }

    public static CreateServiceResponse ToResponse(this Service service)
    {
        return new CreateServiceResponse(
            service.Id,
            service.Name,
            service.Description,
            service.DurationMinutes,
            service.Price,
            service.IsActive
        );
    }
}
