namespace PhysioBook.Features.Articles.Commands.DeleteArticle;

public static class DeleteArticleEndpoint
{
    public static IEndpointRouteBuilder MapDeleteArticleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/articles/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = id.ToCommand();
                var response = await mediator.SendAsync<DeleteArticleCommand, DeleteArticleResponse>(command, ct);
                return Results.Ok(response);
            }))
            .RequireAuthorization(AppPolicies.Admin)
            .WithTags("Articles");

        return app;
    }
}

