namespace PhysioBook.Api.Features.Articles.Queries.GetArticles;

public static class GetArticlesEndpoint
{
    public static IEndpointRouteBuilder MapGetArticlesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/articles", async (
                [AsParameters] GetArticlesRequest request,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var query = request.ToQuery();
                var response = await mediator.SendAsync<GetArticlesQuery, GetArticlesResponse>(query, cancellationToken);
                return Results.Ok(response);
            }))
            .WithName("GetArticles")
            .WithTags("Articles")
            .AllowAnonymous()
            .Produces<GetArticlesResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        return app;
    }
}
