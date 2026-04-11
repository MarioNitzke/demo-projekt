import { Link } from "react-router-dom";

export default function NotFoundPage() {
  return (
    <div className="rounded-3xl bg-white p-8 shadow-sm">
      <h1 className="text-3xl font-bold">Page not found</h1>
      <p className="mt-2 text-sm text-slate-500">The route you requested does not exist in this demo.</p>
      <Link className="mt-4 inline-block rounded-xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white" to="/">
        Back home
      </Link>
    </div>
  );
}
