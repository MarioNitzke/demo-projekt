namespace PhysioBook.Features.Auth.Commands.Login;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (LoginRequest request, IMediator mediator, CancellationToken ct) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();
                var response = await mediator.SendAsync<LoginCommand, LoginResponse>(command, ct);
                return Results.Ok(response);
            }))
            .AllowAnonymous()
            .WithTags("Auth");

        return app;
    }
}

