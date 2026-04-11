namespace PhysioBook.Api.Features.Auth.Commands.Register;

public static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/register", async (
                RegisterRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();
                var response = await mediator.SendAsync<RegisterCommand, RegisterResponse>(command, cancellationToken);
                return Results.Ok(response);
            }))
            .WithName("Register")
            .WithTags("Auth")
            .AllowAnonymous()
            .Produces<RegisterResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }
}
