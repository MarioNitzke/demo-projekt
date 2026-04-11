namespace PhysioBook.Features.Articles.Commands.UpdateArticle;

public static class UpdateArticleEndpoint
{
    public static IEndpointRouteBuilder MapUpdateArticleEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/articles/{id:guid}", async (Guid id, UpdateArticleRequest request, IMediator mediator, CancellationToken ct) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var command = request.ToCommand(id);
                var response = await mediator.SendAsync<UpdateArticleCommand, UpdateArticleResponse>(command, ct);
                return Results.Ok(response);
            }))
            .RequireAuthorization(AppPolicies.Admin)
            .WithTags("Articles");

        return app;
    }
}

