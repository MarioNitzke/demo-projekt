import { Link } from 'react-router-dom';
import { useArticles } from '../features/articles/hooks';

export function ArticlesListPage() {
  const { items, loading, error } = useArticles(1, 20);

  if (loading) {
    return <p>Loading articles...</p>;
  }

  if (error) {
    return <p className="text-red-600">{error}</p>;
  }

  return (
    <section className="space-y-3">
      <h1 className="text-2xl font-semibold">Articles</h1>
      {items.length === 0 ? <p>No articles yet.</p> : null}
      {items.map((article) => (
        <article key={article.id} className="rounded border bg-white p-4">
          <h2 className="text-lg font-semibold">{article.title}</h2>
          <p className="line-clamp-2 text-slate-700">{article.content}</p>
          <div className="mt-3 flex gap-2 text-sm">
            <Link className="text-blue-600" to={`/articles/${article.id}`}>
              Detail
            </Link>
            <Link className="text-blue-600" to={`/articles/${article.id}/edit`}>
              Edit
            </Link>
          </div>
        </article>
      ))}
    </section>
  );
}

