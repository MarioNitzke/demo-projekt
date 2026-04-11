using PhysioBook.Exceptions;

namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public static class GetArticleByIdEndpoint
{
    public static RouteGroupBuilder MapGetArticleByIdEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
        {
            return await ApiResultHelper.ExecuteWithErrorHandling(async () =>
            {
                var query = new GetArticleByIdRequest(id).ToQuery();

                var response = await mediator.SendAsync<GetArticleByIdQuery, GetArticleByIdResponse>(query, ct);

                return Results.Ok(response);
            });
        })
        .AllowAnonymous()
        .Produces<GetArticleByIdResponse>()
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithName("GetArticleById");

        return group;
    }
}
