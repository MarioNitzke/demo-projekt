import { BaseApiService } from '../../shared/BaseApiService';
import type { Article, ArticlesResponse } from './types';

export const ArticlesService = {
  list: (pageNumber = 1, pageSize = 10) =>
    BaseApiService.get<ArticlesResponse>(`/articles?pageNumber=${pageNumber}&pageSize=${pageSize}`),
  byId: (id: string) => BaseApiService.get<Article>(`/articles/${id}`),
  create: (payload: { title: string; content: string }) => BaseApiService.post<Article>('/articles', payload),
  update: (id: string, payload: { title: string; content: string }) => BaseApiService.put<Article>(`/articles/${id}`, payload),
  remove: (id: string) => BaseApiService.delete<{ id: string; deleted: boolean }>(`/articles/${id}`)
};

