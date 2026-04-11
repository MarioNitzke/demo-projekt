import { Link } from "react-router-dom";

export default function HomePage() {
  return (
    <div className="grid gap-8 md:grid-cols-2">
      <section className="rounded-3xl bg-white p-8 shadow-sm">
        <p className="text-sm font-semibold uppercase tracking-wide text-slate-500">Demo scaffold</p>
        <h1 className="mt-3 text-4xl font-bold text-slate-900">PhysioBook</h1>
        <p className="mt-4 text-base leading-7 text-slate-600">
          Reservation system scaffold for physiotherapists. This phase contains authentication,
          Articles CRUD and a dockerized full-stack setup ready for future booking and Stripe slices.
        </p>
        <div className="mt-6 flex gap-3">
          <Link
            to="/articles"
            className="rounded-xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white"
          >
            Browse articles
          </Link>
          <Link
            to="/register"
            className="rounded-xl border border-slate-300 px-5 py-3 text-sm font-semibold text-slate-900"
          >
            Create demo account
          </Link>
        </div>
      </section>

      <section className="rounded-3xl bg-slate-900 p-8 text-white shadow-sm">
        <h2 className="text-2xl font-bold">Seeded demo admin</h2>
        <div className="mt-4 space-y-2 text-sm text-slate-200">
          <p>Email: admin@physiobook.demo</p>
          <p>Password: Admin123!</p>
          <p>Role: Admin</p>
        </div>
        <p className="mt-6 text-sm leading-6 text-slate-300">
          Registration endpoint in this scaffold also assigns the Admin role so it is easy to demo
          article creation from the UI.
        </p>
      </section>
    </div>
  );
}
