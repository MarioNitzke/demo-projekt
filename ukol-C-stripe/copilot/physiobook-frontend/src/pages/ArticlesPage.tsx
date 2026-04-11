import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { useArticles } from '@/features/articles/hooks';
import Layout from '@/components/Layout';

export default function ArticlesPage() {
  const [page, setPage] = useState(1);
  const pageSize = 9;
  const { data, loading, error } = useArticles(page, pageSize);
  const { isAdmin } = useAuth();

  const formatDate = (dateStr: string) => {
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
    });
  };

  const getExcerpt = (content: string, maxLength: number = 150) => {
    if (content.length <= maxLength) return content;
    return content.substring(0, maxLength).trimEnd() + '...';
  };

  return (
    <Layout>
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Articles</h1>
          <p className="text-gray-500 mt-1">
            Expert physiotherapy knowledge and insights
          </p>
        </div>
        {isAdmin && (
          <Link
            to="/articles/create"
            className="inline-flex items-center gap-2 px-5 py-2.5 bg-teal-600 text-white font-medium rounded-lg hover:bg-teal-700 transition-colors shadow-sm"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-5 w-5"
              viewBox="0 0 20 20"
              fill="currentColor"
            >
              <path
                fillRule="evenodd"
                d="M10 3a1 1 0 011 1v5h5a1 1 0 110 2h-5v5a1 1 0 11-2 0v-5H4a1 1 0 110-2h5V4a1 1 0 011-1z"
                clipRule="evenodd"
              />
            </svg>
            Create Article
          </Link>
        )}
      </div>

      {loading && (
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
      )}

      {error && (
        <div className="p-6 bg-red-50 border border-red-200 rounded-xl text-red-700 text-center">
          <p className="font-medium">Failed to load articles</p>
          <p className="text-sm mt-1">{error}</p>
        </div>
      )}

      {!loading && !error && data && (
        <>
          {data.items.length === 0 ? (
            <div className="text-center py-20">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                className="h-16 w-16 text-gray-300 mx-auto mb-4"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
              >
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={1}
                  d="M19 20H5a2 2 0 01-2-2V6a2 2 0 012-2h10a2 2 0 012 2v1m2 13a2 2 0 01-2-2V7m2 13a2 2 0 002-2V9a2 2 0 00-2-2h-2m-4-3H9M7 16h6M7 8h6v4H7V8z"
                />
              </svg>
              <p className="text-gray-500 text-lg">No articles yet</p>
              <p className="text-gray-400 text-sm mt-1">Check back later for new content</p>
            </div>
          ) : (
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
              {data.items.map((article) => (
                <Link
                  key={article.id}
                  to={`/articles/${article.id}`}
                  className="group bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md hover:border-teal-200 transition-all"
                >
                  <div className="p-6">
                    <div className="text-xs text-gray-400 mb-3">
                      {formatDate(article.createdAt)}
                    </div>
                    <h2 className="text-lg font-semibold text-gray-900 group-hover:text-teal-600 transition-colors mb-3 line-clamp-2">
                      {article.title}
                    </h2>
                    <p className="text-gray-600 text-sm leading-relaxed">
                      {getExcerpt(article.content)}
                    </p>
                    <div className="mt-4 inline-flex items-center text-teal-600 text-sm font-medium">
                      Read more
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className="ml-1 h-4 w-4 group-hover:translate-x-1 transition-transform"
                        viewBox="0 0 20 20"
                        fill="currentColor"
                      >
                        <path
                          fillRule="evenodd"
                          d="M10.293 3.293a1 1 0 011.414 0l6 6a1 1 0 010 1.414l-6 6a1 1 0 01-1.414-1.414L14.586 11H3a1 1 0 110-2h11.586l-4.293-4.293a1 1 0 010-1.414z"
                          clipRule="evenodd"
                        />
                      </svg>
                    </div>
                  </div>
                </Link>
              ))}
            </div>
          )}

          {/* Pagination */}
          {data.totalPages > 1 && (
            <div className="flex justify-center items-center gap-2 mt-10">
              <button
                onClick={() => setPage((p) => Math.max(1, p - 1))}
                disabled={page === 1}
                className="px-4 py-2 text-sm font-medium rounded-lg border border-gray-300 text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Previous
              </button>
              {Array.from({ length: data.totalPages }, (_, i) => i + 1).map((p) => (
                <button
                  key={p}
                  onClick={() => setPage(p)}
                  className={`px-4 py-2 text-sm font-medium rounded-lg transition-colors ${
                    p === page
                      ? 'bg-teal-600 text-white'
                      : 'border border-gray-300 text-gray-700 hover:bg-gray-50'
                  }`}
                >
                  {p}
                </button>
              ))}
              <button
                onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
                disabled={page === data.totalPages}
                className="px-4 py-2 text-sm font-medium rounded-lg border border-gray-300 text-gray-700 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
              >
                Next
              </button>
            </div>
          )}
        </>
      )}
    </Layout>
  );
}
