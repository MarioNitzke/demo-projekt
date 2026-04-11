using PhysioBook.Exceptions;

namespace PhysioBook.Features.Articles.Queries.GetArticles;

public static class GetArticlesEndpoint
{
    public static RouteGroupBuilder MapGetArticlesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async ([AsParameters] GetArticlesRequest request, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var query = request.ToQuery();

                var response = await mediator.SendAsync<GetArticlesQuery, GetArticlesResponse>(query, ct);

                return Results.Ok(response);
            });
        })
        .AllowAnonymous()
        .Produces<GetArticlesResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .WithName("GetArticles");

        return group;
    }
}
