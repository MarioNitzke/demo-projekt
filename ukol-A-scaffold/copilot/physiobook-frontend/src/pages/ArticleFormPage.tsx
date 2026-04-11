import { FormEvent, useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { ArticlesService } from '../features/articles/service';
import { useToast } from '../contexts/ToastContext';

export function ArticleFormPage({ mode }: { mode: 'create' | 'edit' }) {
  const { id } = useParams();
  const navigate = useNavigate();
  const { showToast } = useToast();
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');

  useEffect(() => {
    if (mode !== 'edit' || !id) {
      return;
    }

    const run = async () => {
      try {
        const data = await ArticlesService.byId(id);
        setTitle(data.title);
        setContent(data.content);
      } catch (e) {
        showToast((e as Error).message);
      }
    };

    void run();
  }, [mode, id, showToast]);

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();

    try {
      if (mode === 'create') {
        const created = await ArticlesService.create({ title, content });
        showToast('Article created.');
        navigate(`/articles/${created.id}`);
      } else if (id) {
        const updated = await ArticlesService.update(id, { title, content });
        showToast('Article updated.');
        navigate(`/articles/${updated.id}`);
      }
    } catch (error) {
      showToast((error as Error).message);
    }
  };

  return (
    <form onSubmit={onSubmit} className="space-y-3 rounded border bg-white p-4">
      <h1 className="text-xl font-semibold">{mode === 'create' ? 'Create article' : 'Edit article'}</h1>
      <input className="w-full rounded border p-2" value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Title" />
      <textarea className="min-h-40 w-full rounded border p-2" value={content} onChange={(e) => setContent(e.target.value)} placeholder="Content" />
      <button type="submit" className="rounded bg-slate-900 px-4 py-2 text-white">
        Save
      </button>
    </form>
  );
}

