import { Suspense, lazy } from 'react';
import { Navigate, Route, Routes } from 'react-router-dom';
import { Header } from './components/Header';
import { Footer } from './components/Footer';
import { ProtectedRoute } from './components/ProtectedRoute';

const LoginPage = lazy(() => import('./pages/LoginPage').then((m) => ({ default: m.LoginPage })));
const RegisterPage = lazy(() => import('./pages/RegisterPage').then((m) => ({ default: m.RegisterPage })));
const ArticlesListPage = lazy(() => import('./pages/ArticlesListPage').then((m) => ({ default: m.ArticlesListPage })));
const ArticleDetailPage = lazy(() => import('./pages/ArticleDetailPage').then((m) => ({ default: m.ArticleDetailPage })));
const ArticleFormPage = lazy(() => import('./pages/ArticleFormPage').then((m) => ({ default: m.ArticleFormPage })));
const NotFoundPage = lazy(() => import('./pages/NotFoundPage').then((m) => ({ default: m.NotFoundPage })));

export function App() {
  return (
    <div className="min-h-screen flex flex-col">
      <Header />
      <main className="mx-auto w-full max-w-4xl flex-1 p-4">
        <Suspense fallback={<p>Loading...</p>}>
          <Routes>
            <Route path="/" element={<Navigate to="/articles" replace />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/articles" element={<ArticlesListPage />} />
            <Route path="/articles/:id" element={<ArticleDetailPage />} />
            <Route
              path="/articles/new"
              element={
                <ProtectedRoute>
                  <ArticleFormPage mode="create" />
                </ProtectedRoute>
              }
            />
            <Route
              path="/articles/:id/edit"
              element={
                <ProtectedRoute>
                  <ArticleFormPage mode="edit" />
                </ProtectedRoute>
              }
            />
            <Route path="*" element={<NotFoundPage />} />
          </Routes>
        </Suspense>
      </main>
      <Footer />
    </div>
  );
}

