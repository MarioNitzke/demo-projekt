namespace PhysioBook.Api.Features.Auth.Commands.Login;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
                LoginRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();
                var response = await mediator.SendAsync<LoginCommand, LoginResponse>(command, cancellationToken);
                return Results.Ok(response);
            }))
            .WithName("Login")
            .WithTags("Auth")
            .AllowAnonymous()
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
