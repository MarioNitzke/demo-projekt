namespace PhysioBook.Features.Articles.Queries.GetArticles;

public record GetArticlesRequest(int PageNumber = 1, int PageSize = 10);
