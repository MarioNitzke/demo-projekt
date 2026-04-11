import { Link } from 'react-router-dom';

export function NotFoundPage() {
  return (
    <section className="rounded border bg-white p-6 text-center">
      <h1 className="text-xl font-semibold">Page not found</h1>
      <Link to="/articles" className="mt-3 inline-block text-blue-600">
        Go to articles
      </Link>
    </section>
  );
}

