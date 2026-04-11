import type { PagedResponse } from '@/shared/ApiResponse';

export interface Article {
  id: string;
  title: string;
  content: string;
  createdAt: string;
  updatedAt: string;
  authorId: string;
}

export interface CreateArticleRequest {
  title: string;
  content: string;
}

export interface UpdateArticleRequest {
  title: string;
  content: string;
}

export type ArticlesPagedResponse = PagedResponse<Article>;
