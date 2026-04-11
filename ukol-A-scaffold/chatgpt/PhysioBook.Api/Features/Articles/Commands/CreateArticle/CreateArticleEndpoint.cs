namespace PhysioBook.Api.Features.Articles.Commands.CreateArticle;

public static class CreateArticleEndpoint
{
    public static IEndpointRouteBuilder MapCreateArticleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/articles", async (
                CreateArticleRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var command = request.ToCommand(userId);
                var response = await mediator.SendAsync<CreateArticleCommand, CreateArticleResponse>(command, cancellationToken);
                return Results.Created($"/api/articles/{response.Id}", response);
            }))
            .WithName("CreateArticle")
            .WithTags("Articles")
            .RequireAuthorization(AppPolicies.Admin)
            .Produces<CreateArticleResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return app;
    }
}
