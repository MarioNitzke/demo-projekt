namespace PhysioBook.Api.Features.Auth.Commands.RefreshToken;

public static class RefreshTokenEndpoint
{
    public static IEndpointRouteBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/refresh-token", async (
                RefreshTokenRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand();
                var response = await mediator.SendAsync<RefreshTokenCommand, RefreshTokenResponse>(command, cancellationToken);
                return Results.Ok(response);
            }))
            .WithName("RefreshToken")
            .WithTags("Auth")
            .AllowAnonymous()
            .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
