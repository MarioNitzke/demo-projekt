import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { useServices } from '@/features/services/hooks';
import Layout from '@/components/Layout';

export default function ServicesPage() {
  const [page, setPage] = useState(1);
  const pageSize = 9;
  const { data, loading, error } = useServices(page, pageSize);
  const { isAuthenticated } = useAuth();

  return (
    <Layout>
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Nase sluzby</h1>
        <p className="text-gray-500 mt-1">
          Vyberte si sluzbu a objednejte se online
        </p>
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
          <p className="font-medium">Nepodarilo se nacist sluzby</p>
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
                  d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10"
                />
              </svg>
              <p className="text-gray-500 text-lg">Zadne sluzby nejsou k dispozici</p>
              <p className="text-gray-400 text-sm mt-1">Zkuste to prosim pozdeji</p>
            </div>
          ) : (
            <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
              {data.items.map((service) => (
                <div
                  key={service.id}
                  className="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md hover:border-teal-200 transition-all flex flex-col"
                >
                  <div className="p-6 flex-1">
                    <div className="w-10 h-10 bg-teal-100 rounded-lg flex items-center justify-center mb-4">
                      <svg
                        xmlns="http://www.w3.org/2000/svg"
                        className="h-5 w-5 text-teal-600"
                        viewBox="0 0 20 20"
                        fill="currentColor"
                      >
                        <path
                          fillRule="evenodd"
                          d="M3.172 5.172a4 4 0 015.656 0L10 6.343l1.172-1.171a4 4 0 115.656 5.656L10 17.657l-6.828-6.829a4 4 0 010-5.656z"
                          clipRule="evenodd"
                        />
                      </svg>
                    </div>
                    <h2 className="text-lg font-semibold text-gray-900 mb-2">
                      {service.name}
                    </h2>
                    <p className="text-gray-600 text-sm leading-relaxed mb-4">
                      {service.description}
                    </p>
                    <div className="flex items-center gap-4 text-sm text-gray-500">
                      <span className="inline-flex items-center gap-1">
                        <svg
                          xmlns="http://www.w3.org/2000/svg"
                          className="h-4 w-4"
                          viewBox="0 0 20 20"
                          fill="currentColor"
                        >
                          <path
                            fillRule="evenodd"
                            d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-12a1 1 0 10-2 0v4a1 1 0 00.293.707l2.828 2.829a1 1 0 101.415-1.415L11 9.586V6z"
                            clipRule="evenodd"
                          />
                        </svg>
                        {service.durationMinutes} min
                      </span>
                      <span className="font-semibold text-teal-700">
                        {service.price} Kc
                      </span>
                    </div>
                  </div>
                  <div className="px-6 pb-6">
                    {isAuthenticated ? (
                      <Link
                        to={`/booking/${service.id}`}
                        className="block w-full text-center px-5 py-2.5 bg-teal-600 text-white font-medium rounded-lg hover:bg-teal-700 transition-colors shadow-sm"
                      >
                        Objednat se
                      </Link>
                    ) : (
                      <Link
                        to="/login"
                        className="block w-full text-center px-5 py-2.5 bg-teal-600 text-white font-medium rounded-lg hover:bg-teal-700 transition-colors shadow-sm"
                      >
                        Prihlaste se pro objednani
                      </Link>
                    )}
                  </div>
                </div>
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
                Predchozi
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
                Dalsi
              </button>
            </div>
          )}
        </>
      )}
    </Layout>
  );
}
