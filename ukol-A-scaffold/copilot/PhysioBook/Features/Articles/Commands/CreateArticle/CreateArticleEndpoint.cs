namespace PhysioBook.Features.Articles.Commands.CreateArticle;

public static class CreateArticleEndpoint
{
    public static IEndpointRouteBuilder MapCreateArticleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/articles", async (CreateArticleRequest request, IMediator mediator, HttpContext httpContext, CancellationToken ct) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var command = request.ToCommand(userId);
                var response = await mediator.SendAsync<CreateArticleCommand, CreateArticleResponse>(command, ct);
                return Results.Created($"/api/articles/{response.Id}", response);
            }))
            .RequireAuthorization(AppPolicies.Admin)
            .WithTags("Articles");

        return app;
    }
}

