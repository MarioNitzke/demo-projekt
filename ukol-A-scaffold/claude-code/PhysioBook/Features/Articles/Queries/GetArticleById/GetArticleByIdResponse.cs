namespace PhysioBook.Features.Articles.Queries.GetArticleById;

public record GetArticleByIdResponse(
    Guid Id,
    string Title,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? AuthorId);
