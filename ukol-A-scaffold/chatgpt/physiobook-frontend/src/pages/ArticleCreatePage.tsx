import { useState } from "react";
import { useNavigate } from "react-router-dom";
import articleService from "../features/articles/articleService";
import { useToast } from "../contexts/ToastContext";

export default function ArticleCreatePage() {
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);

    try {
      const response = await articleService.createArticle({ title, content });
      showToast("Article created.", "success");
      navigate(`/articles/${response.id}`);
    } catch (error) {
      showToast((error as Error).message, "error");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="mx-auto max-w-3xl rounded-3xl bg-white p-8 shadow-sm">
      <h1 className="text-3xl font-bold">Create article</h1>
      <p className="mt-2 text-sm text-slate-500">Protected admin operation backed by the CreateArticle slice.</p>

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
          {isSubmitting ? "Saving..." : "Create article"}
        </button>
      </form>
    </div>
  );
}
