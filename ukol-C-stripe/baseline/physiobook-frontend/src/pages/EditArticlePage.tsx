import React, { useState, useEffect } from 'react';
import { Link, useParams, useNavigate } from 'react-router-dom';
import { useToast } from '@/contexts/ToastContext';
import { useArticle, useUpdateArticle } from '@/features/articles/hooks';
import Layout from '@/components/Layout';
import type { ProblemDetails } from '@/shared/ApiResponse';
import axios from 'axios';

export default function EditArticlePage() {
  const { id } = useParams<{ id: string }>();
  const { article, loading: fetching, error: fetchError } = useArticle(id);
  const { updateArticle, loading: updating } = useUpdateArticle();
  const { showToast } = useToast();
  const navigate = useNavigate();

  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [errors, setErrors] = useState<Record<string, string[]>>({});
  const [generalError, setGeneralError] = useState('');

  useEffect(() => {
    if (article) {
      setTitle(article.title);
      setContent(article.content);
    }
  }, [article]);

  const validate = (): boolean => {
    const newErrors: Record<string, string[]> = {};
    if (!title.trim()) newErrors.title = ['Title is required'];
    if (!content.trim()) newErrors.content = ['Content is required'];
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setGeneralError('');

    if (!validate() || !id) return;

    try {
      const updated = await updateArticle(id, {
        title: title.trim(),
        content: content.trim(),
      });
      if (updated) {
        showToast('Article updated successfully!', 'success');
        navigate(`/articles/${id}`);
      }
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data) {
        const problem = err.response.data as ProblemDetails;
        if (problem.errors) {
          setErrors(problem.errors);
        } else if (problem.detail) {
          setGeneralError(problem.detail);
        } else {
          setGeneralError('Failed to update article');
        }
      } else {
        setGeneralError('An unexpected error occurred');
      }
    }
  };

  if (fetching) {
    return (
      <Layout>
        <div className="flex justify-center items-center py-20">
          <svg
            className="animate-spin h-8 w-8 text-teal-600"
            xmlns="http://www.w3.org/2000/svg"
            fill="none"
            viewBox="0 0 24 24"
          >
            <circle
              className="opacity-25"
              cx="12"
              cy="12"
              r="10"
              stroke="currentColor"
              strokeWidth="4"
            />
            <path
              className="opacity-75"
              fill="currentColor"
              d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
            />
          </svg>
        </div>
      </Layout>
    );
  }

  if (fetchError || !article) {
    return (
      <Layout>
        <div className="text-center py-20">
          <div className="p-6 bg-red-50 border border-red-200 rounded-xl text-red-700 inline-block">
            <p className="font-medium">Article not found</p>
            <p className="text-sm mt-1">{fetchError || 'Unable to load article for editing.'}</p>
          </div>
          <div className="mt-6">
            <Link
              to="/articles"
              className="text-teal-600 hover:text-teal-700 font-medium inline-flex items-center gap-1"
            >
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-4 w-4"
                viewBox="0 0 20 20"
                fill="currentColor"
              >
                <path
                  fillRule="evenodd"
                  d="M9.707 16.707a1 1 0 01-1.414 0l-6-6a1 1 0 010-1.414l6-6a1 1 0 011.414 1.414L5.414 9H17a1 1 0 110 2H5.414l4.293 4.293a1 1 0 010 1.414z"
                  clipRule="evenodd"
                />
              </svg>
              Back to Articles
            </Link>
          </div>
        </div>
      </Layout>
    );
  }

  return (
    <Layout>
      <div className="max-w-3xl mx-auto">
        <Link
          to={`/articles/${id}`}
          className="inline-flex items-center gap-1 text-gray-500 hover:text-teal-600 text-sm font-medium mb-6 transition-colors"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            className="h-4 w-4"
            viewBox="0 0 20 20"
            fill="currentColor"
          >
            <path
              fillRule="evenodd"
              d="M9.707 16.707a1 1 0 01-1.414 0l-6-6a1 1 0 010-1.414l6-6a1 1 0 011.414 1.414L5.414 9H17a1 1 0 110 2H5.414l4.293 4.293a1 1 0 010 1.414z"
              clipRule="evenodd"
            />
          </svg>
          Back to Article
        </Link>

        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8">
          <h1 className="text-2xl font-bold text-gray-900 mb-6">Edit Article</h1>

          {generalError && (
            <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
              {generalError}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-6">
            <div>
              <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
                Title
              </label>
              <input
                id="title"
                type="text"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                className={`w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow ${
                  errors.title ? 'border-red-300 bg-red-50' : 'border-gray-300'
                }`}
                placeholder="Enter article title"
              />
              {errors.title?.map((msg, i) => (
                <p key={i} className="mt-1 text-sm text-red-600">{msg}</p>
              ))}
            </div>

            <div>
              <label htmlFor="content" className="block text-sm font-medium text-gray-700 mb-1">
                Content
              </label>
              <textarea
                id="content"
                value={content}
                onChange={(e) => setContent(e.target.value)}
                rows={15}
                className={`w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow resize-y ${
                  errors.content ? 'border-red-300 bg-red-50' : 'border-gray-300'
                }`}
                placeholder="Write your article content here..."
              />
              {errors.content?.map((msg, i) => (
                <p key={i} className="mt-1 text-sm text-red-600">{msg}</p>
              ))}
            </div>

            <div className="flex justify-end gap-3 pt-4">
              <Link
                to={`/articles/${id}`}
                className="px-6 py-2.5 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Cancel
              </Link>
              <button
                type="submit"
                disabled={updating}
                className="px-6 py-2.5 text-sm font-medium text-white bg-teal-600 rounded-lg hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                {updating ? (
                  <span className="inline-flex items-center gap-2">
                    <svg
                      className="animate-spin h-4 w-4"
                      xmlns="http://www.w3.org/2000/svg"
                      fill="none"
                      viewBox="0 0 24 24"
                    >
                      <circle
                        className="opacity-25"
                        cx="12"
                        cy="12"
                        r="10"
                        stroke="currentColor"
                        strokeWidth="4"
                      />
                      <path
                        className="opacity-75"
                        fill="currentColor"
                        d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                      />
                    </svg>
                    Saving...
                  </span>
                ) : (
                  'Save Changes'
                )}
              </button>
            </div>
          </form>
        </div>
      </div>
    </Layout>
  );
}
