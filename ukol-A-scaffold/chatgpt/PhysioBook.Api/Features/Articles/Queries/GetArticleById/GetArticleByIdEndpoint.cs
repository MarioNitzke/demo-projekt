namespace PhysioBook.Api.Features.Articles.Queries.GetArticleById;

public static class GetArticleByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetArticleByIdEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/articles/{id:guid}", async (
                Guid id,
                IMediator mediator,
                CancellationToken cancellationToken) =>
            await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var request = new GetArticleByIdRequest { Id = id };
                var query = request.ToQuery();
                var response = await mediator.SendAsync<GetArticleByIdQuery, GetArticleByIdResponse>(query, cancellationToken);
                return Results.Ok(response);
            }))
            .WithName("GetArticleById")
            .WithTags("Articles")
            .AllowAnonymous()
            .Produces<GetArticleByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        return app;
    }
}
