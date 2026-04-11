import React, { Suspense, lazy } from 'react';
import { Routes, Route } from 'react-router-dom';
import { AuthProvider } from '@/contexts/AuthContext';
import { ToastProvider } from '@/contexts/ToastContext';
import ProtectedRoute from '@/components/ProtectedRoute';

const HomePage = lazy(() => import('@/pages/HomePage'));
const LoginPage = lazy(() => import('@/pages/LoginPage'));
const RegisterPage = lazy(() => import('@/pages/RegisterPage'));
const ArticlesPage = lazy(() => import('@/pages/ArticlesPage'));
const ArticleDetailPage = lazy(() => import('@/pages/ArticleDetailPage'));
const CreateArticlePage = lazy(() => import('@/pages/CreateArticlePage'));
const EditArticlePage = lazy(() => import('@/pages/EditArticlePage'));

function LoadingFallback() {
  return (
    <div className="min-h-screen flex items-center justify-center">
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
  );
}

export default function App() {
  return (
    <AuthProvider>
      <ToastProvider>
        <Suspense fallback={<LoadingFallback />}>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/articles" element={<ArticlesPage />} />
            <Route
              path="/articles/create"
              element={
                <ProtectedRoute>
                  <CreateArticlePage />
                </ProtectedRoute>
              }
            />
            <Route path="/articles/:id" element={<ArticleDetailPage />} />
            <Route
              path="/articles/:id/edit"
              element={
                <ProtectedRoute>
                  <EditArticlePage />
                </ProtectedRoute>
              }
            />
          </Routes>
        </Suspense>
      </ToastProvider>
    </AuthProvider>
  );
}
