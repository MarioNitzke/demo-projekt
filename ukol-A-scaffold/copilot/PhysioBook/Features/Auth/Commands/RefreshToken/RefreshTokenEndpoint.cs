namespace PhysioBook.Features.Auth.Commands.RefreshToken;

public static class RefreshTokenEndpoint
{
    public static IEndpointRouteBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh-token", async (RefreshTokenRequest request, IMediator mediator, CancellationToken ct) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();
                var response = await mediator.SendAsync<RefreshTokenCommand, RefreshTokenResponse>(command, ct);
                return Results.Ok(response);
            }))
            .AllowAnonymous()
            .WithTags("Auth");

        return app;
    }
}

