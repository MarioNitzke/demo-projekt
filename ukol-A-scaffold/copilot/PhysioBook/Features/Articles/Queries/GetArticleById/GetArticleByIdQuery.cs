namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public sealed class GetArticleByIdQuery : IQuery<GetArticleByIdResponse>
{
    public Guid Id { get; set; }
}

