namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public static class GetArticleByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetArticleByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/articles/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var request = new GetArticleByIdRequest(id);
                var query = request.ToQuery();
                var response = await mediator.SendAsync<GetArticleByIdQuery, GetArticleByIdResponse>(query, ct);
                return Results.Ok(response);
            }))
            .AllowAnonymous()
            .WithTags("Articles");

        return app;
    }
}

