namespace PhysioBook.Api.Features.Articles.Commands.DeleteArticle;

public static class DeleteArticleEndpoint
{
    public static IEndpointRouteBuilder MapDeleteArticleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/articles/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var request = new DeleteArticleRequest { Id = id };
                var command = request.ToCommand();
                var response = await mediator.SendAsync<DeleteArticleCommand, DeleteArticleResponse>(command, cancellationToken);
                return Results.Ok(response);
            }))
            .WithName("DeleteArticle")
            .WithTags("Articles")
            .RequireAuthorization(AppPolicies.Admin)
            .Produces<DeleteArticleResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
