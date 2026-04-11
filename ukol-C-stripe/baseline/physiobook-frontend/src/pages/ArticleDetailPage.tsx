import React, { useState } from 'react';
import { Link, useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { useToast } from '@/contexts/ToastContext';
import { useArticle, useDeleteArticle } from '@/features/articles/hooks';
import Layout from '@/components/Layout';

export default function ArticleDetailPage() {
  const { id } = useParams<{ id: string }>();
  const { article, loading, error } = useArticle(id);
  const { deleteArticle, loading: deleting } = useDeleteArticle();
  const { isAdmin } = useAuth();
  const { showToast } = useToast();
  const navigate = useNavigate();
  const [showDeleteConfirm, setShowDeleteConfirm] = useState(false);

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const handleDelete = async () => {
    if (!id) return;
    const success = await deleteArticle(id);
    if (success) {
      showToast('Article deleted successfully', 'success');
      navigate('/articles');
    } else {
      showToast('Failed to delete article', 'error');
    }
    setShowDeleteConfirm(false);
  };

  if (loading) {
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

  if (error || !article) {
    return (
      <Layout>
        <div className="text-center py-20">
          <div className="p-6 bg-red-50 border border-red-200 rounded-xl text-red-700 inline-block">
            <p className="font-medium">Article not found</p>
            <p className="text-sm mt-1">{error || 'The article you are looking for does not exist.'}</p>
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
        {/* Back link */}
        <Link
          to="/articles"
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
          Back to Articles
        </Link>

        <article className="bg-white rounded-xl shadow-sm border border-gray-200 p-8 md:p-10">
          {/* Header */}
          <header className="mb-8">
            <h1 className="text-3xl md:text-4xl font-bold text-gray-900 mb-4 leading-tight">
              {article.title}
            </h1>
            <div className="flex flex-wrap items-center gap-4 text-sm text-gray-500">
              <span className="inline-flex items-center gap-1">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-4 w-4"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fillRule="evenodd"
                    d="M6 2a1 1 0 00-1 1v1H4a2 2 0 00-2 2v10a2 2 0 002 2h12a2 2 0 002-2V6a2 2 0 00-2-2h-1V3a1 1 0 10-2 0v1H7V3a1 1 0 00-1-1zm0 5a1 1 0 000 2h8a1 1 0 100-2H6z"
                    clipRule="evenodd"
                  />
                </svg>
                Published {formatDate(article.createdAt)}
              </span>
              {article.updatedAt !== article.createdAt && (
                <span className="text-gray-400">
                  Updated {formatDate(article.updatedAt)}
                </span>
              )}
            </div>
          </header>

          {/* Content */}
          <div className="prose prose-gray max-w-none text-gray-700 leading-relaxed whitespace-pre-wrap">
            {article.content}
          </div>

          {/* Admin Actions */}
          {isAdmin && (
            <div className="mt-10 pt-6 border-t border-gray-200 flex gap-3">
              <Link
                to={`/articles/${article.id}/edit`}
                className="inline-flex items-center gap-2 px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-4 w-4"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path d="M13.586 3.586a2 2 0 112.828 2.828l-.793.793-2.828-2.828.793-.793zM11.379 5.793L3 14.172V17h2.828l8.38-8.379-2.83-2.828z" />
                </svg>
                Edit
              </Link>
              <button
                onClick={() => setShowDeleteConfirm(true)}
                className="inline-flex items-center gap-2 px-4 py-2 bg-red-600 text-white text-sm font-medium rounded-lg hover:bg-red-700 transition-colors"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-4 w-4"
                  viewBox="0 0 20 20"
                  fill="currentColor"
                >
                  <path
                    fillRule="evenodd"
                    d="M9 2a1 1 0 00-.894.553L7.382 4H4a1 1 0 000 2v10a2 2 0 002 2h8a2 2 0 002-2V6a1 1 0 100-2h-3.382l-.724-1.447A1 1 0 0011 2H9zM7 8a1 1 0 012 0v6a1 1 0 11-2 0V8zm5-1a1 1 0 00-1 1v6a1 1 0 102 0V8a1 1 0 00-1-1z"
                    clipRule="evenodd"
                  />
                </svg>
                Delete
              </button>
            </div>
          )}
        </article>
      </div>

      {/* Delete Confirmation Modal */}
      {showDeleteConfirm && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          <div
            className="absolute inset-0 bg-black/50"
            onClick={() => setShowDeleteConfirm(false)}
          />
          <div className="relative bg-white rounded-xl shadow-xl p-6 max-w-sm w-full">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">Delete Article</h3>
            <p className="text-gray-600 text-sm mb-6">
              Are you sure you want to delete this article? This action cannot be undone.
            </p>
            <div className="flex justify-end gap-3">
              <button
                onClick={() => setShowDeleteConfirm(false)}
                className="px-4 py-2 text-sm font-medium text-gray-700 bg-gray-100 rounded-lg hover:bg-gray-200 transition-colors"
              >
                Cancel
              </button>
              <button
                onClick={handleDelete}
                disabled={deleting}
                className="px-4 py-2 text-sm font-medium text-white bg-red-600 rounded-lg hover:bg-red-700 disabled:opacity-50 transition-colors"
              >
                {deleting ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}
    </Layout>
  );
}
