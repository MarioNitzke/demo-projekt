namespace PhysioBook.Api.Features.Articles.Commands.UpdateArticle;

public static class UpdateArticleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateArticleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/articles/{id:guid}", async (
                Guid id,
                UpdateArticleRequest request,
                HttpContext httpContext,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                var command = request.ToCommand(id, userId);
                var response = await mediator.SendAsync<UpdateArticleCommand, UpdateArticleResponse>(command, cancellationToken);
                return Results.Ok(response);
            }))
            .WithName("UpdateArticle")
            .WithTags("Articles")
            .RequireAuthorization(AppPolicies.Admin)
            .Produces<UpdateArticleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
