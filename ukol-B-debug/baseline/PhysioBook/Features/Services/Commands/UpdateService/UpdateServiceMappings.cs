using PhysioBook.Data.Entities;

namespace PhysioBook.Features.Services.Commands.UpdateService;

public static class UpdateServiceMappings
{
    public static UpdateServiceCommand ToCommand(this UpdateServiceRequest request, Guid id)
    {
        return new UpdateServiceCommand
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price
        };
    }

    public static void ApplyTo(this UpdateServiceCommand command, Service entity)
    {
        entity.Name = command.Name;
        entity.Description = command.Description;
        entity.DurationMinutes = command.DurationMinutes;
        entity.Price = command.Price;
    }

    public static UpdateServiceResponse ToResponse(this Service service)
    {
        return new UpdateServiceResponse(
            service.Id,
            service.Name,
            service.Description,
            service.DurationMinutes,
            service.Price,
            service.IsActive
        );
    }
}
