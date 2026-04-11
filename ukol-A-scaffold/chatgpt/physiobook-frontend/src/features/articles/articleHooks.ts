import { useEffect, useState } from "react";
import articleService from "./articleService";
import type { ArticleDetail, ArticleListItem } from "./articleTypes";

export function useArticles(searchTerm?: string, reloadToken?: number) {
  const [items, setItems] = useState<ArticleListItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let isCancelled = false;
    setIsLoading(true);
    setError(null);

    articleService
      .getArticles(searchTerm)
      .then((response) => {
        if (!isCancelled) {
          setItems(response.items ?? []);
        }
      })
      .catch((err: Error) => {
        if (!isCancelled) {
          setItems([]);
          setError(err.message);
        }
      })
      .finally(() => {
        if (!isCancelled) {
          setIsLoading(false);
        }
      });

    return () => {
      isCancelled = true;
    };
  }, [searchTerm, reloadToken]);

  return { items, isLoading, error };
}

export function useArticle(id?: string) {
  const [item, setItem] = useState<ArticleDetail | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      setIsLoading(false);
      return;
    }

    let isCancelled = false;
    setIsLoading(true);
    setError(null);

    articleService
      .getArticleById(id)
      .then((response) => {
        if (!isCancelled) {
          setItem(response);
        }
      })
      .catch((err: Error) => {
        if (!isCancelled) {
          setItem(null);
          setError(err.message);
        }
      })
      .finally(() => {
        if (!isCancelled) {
          setIsLoading(false);
        }
      });

    return () => {
      isCancelled = true;
    };
  }, [id]);

  return { item, isLoading, error };
}
