export type Article = {
  id: string;
  title: string;
  content: string;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
  userId?: string | null;
};

export type ArticlesResponse = {
  items: Article[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
};

