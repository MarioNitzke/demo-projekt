import { useState, useEffect, useCallback } from 'react';
import { articleService } from './articleService';
import type { Article, ArticlesPagedResponse, CreateArticleRequest, UpdateArticleRequest } from './types';

export function useArticles(page: number = 1, pageSize: number = 10) {
  const [data, setData] = useState<ArticlesPagedResponse | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchArticles = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const response = await articleService.getArticles(page, pageSize);
      setData(response);
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to fetch articles';
      setError(message);
    } finally {
      setLoading(false);
    }
  }, [page, pageSize]);

  useEffect(() => {
    fetchArticles();
  }, [fetchArticles]);

  return { data, loading, error, refetch: fetchArticles };
}

export function useArticle(id: string | undefined) {
  const [article, setArticle] = useState<Article | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      setLoading(false);
      return;
    }

    let cancelled = false;
    const fetchArticle = async () => {
      setLoading(true);
      setError(null);
      try {
        const response = await articleService.getArticleById(id);
        if (!cancelled) {
          setArticle(response);
        }
      } catch (err: unknown) {
        if (!cancelled) {
          const message = err instanceof Error ? err.message : 'Failed to fetch article';
          setError(message);
        }
      } finally {
        if (!cancelled) {
          setLoading(false);
        }
      }
    };

    fetchArticle();
    return () => {
      cancelled = true;
    };
  }, [id]);

  return { article, loading, error };
}

export function useCreateArticle() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const createArticle = useCallback(async (data: CreateArticleRequest): Promise<Article | null> => {
    setLoading(true);
    setError(null);
    try {
      const article = await articleService.createArticle(data);
      return article;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to create article';
      setError(message);
      return null;
    } finally {
      setLoading(false);
    }
  }, []);

  return { createArticle, loading, error };
}

export function useUpdateArticle() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const updateArticle = useCallback(
    async (id: string, data: UpdateArticleRequest): Promise<Article | null> => {
      setLoading(true);
      setError(null);
      try {
        const article = await articleService.updateArticle(id, data);
        return article;
      } catch (err: unknown) {
        const message = err instanceof Error ? err.message : 'Failed to update article';
        setError(message);
        return null;
      } finally {
        setLoading(false);
      }
    },
    [],
  );

  return { updateArticle, loading, error };
}

export function useDeleteArticle() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const deleteArticle = useCallback(async (id: string): Promise<boolean> => {
    setLoading(true);
    setError(null);
    try {
      await articleService.deleteArticle(id);
      return true;
    } catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'Failed to delete article';
      setError(message);
      return false;
    } finally {
      setLoading(false);
    }
  }, []);

  return { deleteArticle, loading, error };
}
