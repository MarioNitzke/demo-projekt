import { Suspense, lazy } from "react";
import { Route, Routes } from "react-router-dom";
import Header from "./components/Header";
import Footer from "./components/Footer";
import ProtectedRoute from "./components/ProtectedRoute";

const HomePage = lazy(() => import("./pages/HomePage"));
const LoginPage = lazy(() => import("./pages/LoginPage"));
const RegisterPage = lazy(() => import("./pages/RegisterPage"));
const ArticlesListPage = lazy(() => import("./pages/ArticlesListPage"));
const ArticleDetailPage = lazy(() => import("./pages/ArticleDetailPage"));
const ArticleCreatePage = lazy(() => import("./pages/ArticleCreatePage"));
const ArticleEditPage = lazy(() => import("./pages/ArticleEditPage"));
const NotFoundPage = lazy(() => import("./pages/NotFoundPage"));

export default function App() {
  return (
    <div className="flex min-h-screen flex-col bg-slate-50 text-slate-900">
      <Header />
      <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
        <Suspense fallback={<div className="rounded-xl bg-white p-6 shadow">Loading…</div>}>
          <Routes>
            <Route path="/" element={<HomePage />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/articles" element={<ArticlesListPage />} />
            <Route path="/articles/:id" element={<ArticleDetailPage />} />
            <Route
              path="/articles/new"
              element={
                <ProtectedRoute>
                  <ArticleCreatePage />
                </ProtectedRoute>
              }
            />
            <Route
              path="/articles/:id/edit"
              element={
                <ProtectedRoute>
                  <ArticleEditPage />
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
