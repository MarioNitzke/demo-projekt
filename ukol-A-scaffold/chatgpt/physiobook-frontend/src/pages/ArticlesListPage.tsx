import { useState } from "react";
import { Link } from "react-router-dom";
import articleService from "../features/articles/articleService";
import { useArticles } from "../features/articles/articleHooks";
import { useAuth } from "../contexts/AuthContext";
import { useToast } from "../contexts/ToastContext";

export default function ArticlesListPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [refreshKey, setRefreshKey] = useState(0);
  const { items, isLoading, error } = useArticles(searchTerm, refreshKey);
  const { isAuthenticated } = useAuth();
  const { showToast } = useToast();

  const handleDelete = async (id: string) => {
    if (!window.confirm("Delete this article?")) {
      return;
    }

    try {
      await articleService.deleteArticle(id);
      showToast("Article deleted.", "success");
      setRefreshKey((value) => value + 1);
    } catch (error) {
      showToast((error as Error).message, "error");
    }
  };

  return (
    <div className="space-y-6">
      <div className="flex flex-col justify-between gap-4 rounded-3xl bg-white p-6 shadow-sm md:flex-row md:items-center">
        <div>
          <h1 className="text-3xl font-bold">Articles</h1>
          <p className="mt-2 text-sm text-slate-500">
            Public article listing with protected create, update and delete actions.
          </p>
        </div>

        {isAuthenticated && (
          <Link
            to="/articles/new"
            className="inline-flex items-center justify-center rounded-xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white"
          >
            New article
          </Link>
        )}
      </div>

      <div className="rounded-3xl bg-white p-6 shadow-sm">
        <input
          className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-900"
          placeholder="Search by title or content"
          value={searchTerm}
          onChange={(event) => setSearchTerm(event.target.value)}
        />
      </div>

      {isLoading ? (
        <div className="rounded-3xl bg-white p-6 shadow-sm">Loading articles…</div>
      ) : error ? (
        <div className="rounded-3xl bg-red-50 p-6 text-sm text-red-700 shadow-sm">
          Failed to load articles: {error}
        </div>
      ) : (
        <div className="grid gap-4">
          {items.map((article) => (
            <article key={article.id} className="rounded-3xl bg-white p-6 shadow-sm">
              <div className="flex flex-col justify-between gap-4 md:flex-row md:items-start">
                <div>
                  <h2 className="text-2xl font-bold">
                    <Link to={`/articles/${article.id}`}>{article.title}</Link>
                  </h2>
                  <p className="mt-2 text-sm leading-6 text-slate-600">{article.preview}</p>
                  <p className="mt-3 text-xs text-slate-400">
                    Updated {new Date(article.updatedAt).toLocaleString()}
                  </p>
                </div>

                <div className="flex gap-2">
                  <Link
                    to={`/articles/${article.id}`}
                    className="rounded-xl border border-slate-300 px-4 py-2 text-sm font-semibold"
                  >
                    Detail
                  </Link>
                  {isAuthenticated && (
                    <>
                      <Link
                        to={`/articles/${article.id}/edit`}
                        className="rounded-xl border border-slate-300 px-4 py-2 text-sm font-semibold"
                      >
                        Edit
                      </Link>
                      <button
                        onClick={() => handleDelete(article.id)}
                        className="rounded-xl bg-red-600 px-4 py-2 text-sm font-semibold text-white"
                      >
                        Delete
                      </button>
                    </>
                  )}
                </div>
              </div>
            </article>
          ))}

          {items.length === 0 && (
            <div className="rounded-3xl bg-white p-6 text-sm text-slate-500 shadow-sm">
              No articles found.
            </div>
          )}
        </div>
      )}
    </div>
  );
}
