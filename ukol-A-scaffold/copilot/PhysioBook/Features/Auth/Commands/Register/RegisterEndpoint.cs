namespace PhysioBook.Features.Auth.Commands.Register;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterRequest request, IMediator mediator, CancellationToken ct) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();
                var response = await mediator.SendAsync<RegisterCommand, RegisterResponse>(command, ct);
                return Results.Ok(response);
            }))
            .AllowAnonymous()
            .WithTags("Auth");

        return app;
    }
}

