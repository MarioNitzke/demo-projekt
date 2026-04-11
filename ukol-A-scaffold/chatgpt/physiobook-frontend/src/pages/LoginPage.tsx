import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";
import { useToast } from "../contexts/ToastContext";

export default function LoginPage() {
  const { login } = useAuth();
  const { showToast } = useToast();
  const navigate = useNavigate();
  const location = useLocation();
  const [email, setEmail] = useState("admin@physiobook.demo");
  const [password, setPassword] = useState("Admin123!");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const redirectTo = (location.state as { from?: string } | null)?.from ?? "/articles";

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setIsSubmitting(true);

    try {
      await login({ email, password });
      showToast("You are now logged in.", "success");
      navigate(redirectTo);
    } catch (error) {
      showToast((error as Error).message, "error");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="mx-auto max-w-lg rounded-3xl bg-white p-8 shadow-sm">
      <h1 className="text-3xl font-bold">Login</h1>
      <p className="mt-2 text-sm text-slate-500">Sign in with the seeded demo admin or your own account.</p>

      <form onSubmit={handleSubmit} className="mt-6 space-y-4">
        <label className="block">
          <span className="mb-2 block text-sm font-medium">Email</span>
          <input
            className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-900"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            type="email"
            required
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-medium">Password</span>
          <input
            className="w-full rounded-xl border border-slate-300 px-4 py-3 outline-none focus:border-slate-900"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            type="password"
            required
          />
        </label>

        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full rounded-xl bg-slate-900 px-5 py-3 text-sm font-semibold text-white disabled:opacity-60"
        >
          {isSubmitting ? "Signing in..." : "Login"}
        </button>
      </form>
    </div>
  );
}
