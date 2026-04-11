import type { PagedResponse } from "../../shared/ApiResponse";

export interface ArticleListItem {
  id: string;
  title: string;
  preview: string;
  createdAt: string;
  updatedAt: string;
  createdByUserId?: string | null;
}

export interface ArticleDetail {
  id: string;
  title: string;
  content: string;
  createdAt: string;
  updatedAt: string;
  createdByUserId?: string | null;
}

export interface ArticleFormRequest {
  title: string;
  content: string;
}

export interface DeleteArticleResponse {
  id: string;
  message: string;
}

export type ArticlesResponse = PagedResponse<ArticleListItem>;
