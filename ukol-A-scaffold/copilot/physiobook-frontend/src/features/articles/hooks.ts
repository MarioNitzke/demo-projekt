import { useEffect, useState } from 'react';
import { ArticlesService } from './service';
import type { Article } from './types';

export function useArticles(pageNumber = 1, pageSize = 10) {
  const [items, setItems] = useState<Article[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const run = async () => {
      try {
        setLoading(true);
        const data = await ArticlesService.list(pageNumber, pageSize);
        setItems(data.items);
      } catch (e) {
        setError((e as Error).message);
      } finally {
        setLoading(false);
      }
    };

    void run();
  }, [pageNumber, pageSize]);

  return { items, loading, error };
}

