import { Link, useParams } from "react-router-dom";
import { useArticle } from "../features/articles/articleHooks";
import { useAuth } from "../contexts/AuthContext";

export default function ArticleDetailPage() {
  const { id } = useParams();
  const { item, isLoading } = useArticle(id);
  const { isAuthenticated } = useAuth();

  if (isLoading) {
    return <div className="rounded-3xl bg-white p-6 shadow-sm">Loading article…</div>;
  }

  if (!item) {
    return (
      <div className="rounded-3xl bg-white p-6 shadow-sm">
        <h1 className="text-2xl font-bold">Article not found</h1>
        <Link className="mt-4 inline-block text-sm font-semibold underline" to="/articles">
          Back to list
        </Link>
      </div>
    );
  }

  return (
    <article className="rounded-3xl bg-white p-8 shadow-sm">
      <div className="flex flex-col justify-between gap-4 md:flex-row md:items-start">
        <div>
          <p className="text-sm font-semibold uppercase tracking-wide text-slate-500">Article detail</p>
          <h1 className="mt-2 text-4xl font-bold">{item.title}</h1>
          <p className="mt-3 text-sm text-slate-400">
            Created {new Date(item.createdAt).toLocaleString()} · Updated {new Date(item.updatedAt).toLocaleString()}
          </p>
        </div>
        <div className="flex gap-2">
          <Link
            to="/articles"
            className="rounded-xl border border-slate-300 px-4 py-2 text-sm font-semibold"
          >
            Back
          </Link>
          {isAuthenticated && (
            <Link
              to={`/articles/${item.id}/edit`}
              className="rounded-xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white"
            >
              Edit
            </Link>
          )}
        </div>
      </div>

      <div className="prose mt-8 max-w-none whitespace-pre-wrap text-slate-700">{item.content}</div>
    </article>
  );
}
