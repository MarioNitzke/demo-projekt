import { useEffect, useState } from 'react';
import { Link, useNavigate, useParams } from 'react-router-dom';
import { ArticlesService } from '../features/articles/service';
import type { Article } from '../features/articles/types';
import { useToast } from '../contexts/ToastContext';

export function ArticleDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [article, setArticle] = useState<Article | null>(null);

  useEffect(() => {
    if (!id) {
      return;
    }

    const run = async () => {
      try {
        const data = await ArticlesService.byId(id);
        setArticle(data);
      } catch (e) {
        showToast((e as Error).message);
      }
    };

    void run();
  }, [id, showToast]);

  const onDelete = async () => {
    if (!id) {
      return;
    }

    try {
      await ArticlesService.remove(id);
      showToast('Article deleted.');
      navigate('/articles');
    } catch (e) {
      showToast((e as Error).message);
    }
  };

  if (!article) {
    return <p>Loading article...</p>;
  }

  return (
    <section className="space-y-4 rounded border bg-white p-4">
      <h1 className="text-2xl font-semibold">{article.title}</h1>
      <p className="whitespace-pre-wrap text-slate-700">{article.content}</p>
      <div className="flex gap-2">
        <Link to={`/articles/${article.id}/edit`} className="rounded bg-slate-900 px-3 py-2 text-white">
          Edit
        </Link>
        <button type="button" onClick={onDelete} className="rounded bg-red-600 px-3 py-2 text-white">
          Delete
        </button>
      </div>
    </section>
  );
}

