import BaseApiService from "../../shared/BaseApiService";
import type {
  ArticleDetail,
  ArticleFormRequest,
  ArticlesResponse,
  DeleteArticleResponse
} from "./articleTypes";

class ArticleService extends BaseApiService {
  getArticles(searchTerm?: string): Promise<ArticlesResponse> {
    const query = searchTerm ? `?searchTerm=${encodeURIComponent(searchTerm)}` : "";
    return this.get<ArticlesResponse>(`/articles${query}`);
  }

  getArticleById(id: string): Promise<ArticleDetail> {
    return this.get<ArticleDetail>(`/articles/${id}`);
  }

  createArticle(request: ArticleFormRequest): Promise<ArticleDetail> {
    return this.post<ArticleDetail>("/articles", request);
  }

  updateArticle(id: string, request: ArticleFormRequest): Promise<ArticleDetail> {
    return this.put<ArticleDetail>(`/articles/${id}`, request);
  }

  deleteArticle(id: string): Promise<DeleteArticleResponse> {
    return this.delete<DeleteArticleResponse>(`/articles/${id}`);
  }
}

const articleService = new ArticleService();
export default articleService;
