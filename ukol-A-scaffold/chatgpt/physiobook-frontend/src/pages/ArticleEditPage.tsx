import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import articleService from "../features/articles/articleService";
import { useArticle } from "../features/articles/articleHooks";
import { useToast } from "../contexts/ToastContext";

export default function ArticleEditPage() {
  const { id } = useParams();
  const { item, isLoading } = useArticle(id);
  const { showToast } = useToast();
  const navigate = useNavigate();
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  useEffect(() => {
    if (item) {
      setTitle(item.title);
      setContent(item.content);
    }
  }, [item]);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    if (!id) {
      return;
    }

    setIsSubmitting(true);

    try {
      const response = await articleService.updateArticle(id, { title, content });
      showToast("Article updated.", "success");
      navigate(`/articles/${response.id}`);
    } catch (error) {
      showToast((error as Error).message, "error");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return <div className="rounded-3xl bg-white p-6 shadow-sm">Loading article…</div>;
  }

  if (!item) {
    return <div className="rounded-3xl bg-white p-6 shadow-sm">Article not found.</div>;
  }

  return (
    <div className="mx-auto max-w-3xl rounded-3xl bg-white p-8 shadow-sm">
      <h1 className="text-3xl font-bold">Edit article</h1>
      <p className="mt-2 text-sm text-slate-500">Protected admin operation backed by the UpdateArticle slice.</p>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        <label className="block">
          <span className="mb-2 block text-sm font-medium">Title</span>
          <input
            className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-900"
            value={title}
            onChange={(event) => setTitle(event.target.value)}
            required
            maxLength={200}
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-medium">Content</span>
          <textarea
            className="min-h-64 w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-900"
            value={content}
            onChange={(event) => setContent(event.target.value)}
            required
            minLength={10}
          />
        </label>

        <button
          type="submit"
          disabled={isSubmitting}
          className="rounded-xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white disabled:opacity-60"
        >
          {isSubmitting ? "Saving..." : "Update article"}
        </button>
      </form>
    </div>
  );
}
