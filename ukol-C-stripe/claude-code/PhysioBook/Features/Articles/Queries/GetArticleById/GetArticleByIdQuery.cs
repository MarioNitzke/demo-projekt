namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public class GetArticleByIdQuery : IQuery<GetArticleByIdResponse>
{
    public Guid Id { get; set; }
}
