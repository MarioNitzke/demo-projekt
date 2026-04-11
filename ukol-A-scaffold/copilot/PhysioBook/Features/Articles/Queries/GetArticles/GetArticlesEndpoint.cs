namespace PhysioBook.Features.Articles.Queries.GetArticles;

public static class GetArticlesEndpoint
{
    public static IEndpointRouteBuilder MapGetArticlesEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/articles", async (int pageNumber, int pageSize, IMediator mediator, CancellationToken ct) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var request = new GetArticlesRequest(pageNumber == 0 ? 1 : pageNumber, pageSize == 0 ? 10 : pageSize);
                var query = request.ToQuery();
                var response = await mediator.SendAsync<GetArticlesQuery, GetArticlesResponse>(query, ct);
                return Results.Ok(response);
            }))
            .AllowAnonymous()
            .WithTags("Articles");

        return app;
    }
}

