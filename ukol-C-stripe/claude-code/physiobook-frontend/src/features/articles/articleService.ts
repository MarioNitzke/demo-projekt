import api from '@/shared/BaseApiService';
import type { Article, CreateArticleRequest, UpdateArticleRequest, ArticlesPagedResponse } from './types';

export const articleService = {
  async getArticles(page: number = 1, pageSize: number = 10): Promise<ArticlesPagedResponse> {
    const response = await api.get<ArticlesPagedResponse>('/articles', {
      params: { pageNumber: page, pageSize },
    });
    return response.data;
  },

  async getArticleById(id: string): Promise<Article> {
    const response = await api.get<Article>(`/articles/${id}`);
    return response.data;
  },

  async createArticle(data: CreateArticleRequest): Promise<Article> {
    const response = await api.post<Article>('/articles', data);
    return response.data;
  },

  async updateArticle(id: string, data: UpdateArticleRequest): Promise<Article> {
    const response = await api.put<Article>(`/articles/${id}`, data);
    return response.data;
  },

  async deleteArticle(id: string): Promise<void> {
    await api.delete(`/articles/${id}`);
  },
};
