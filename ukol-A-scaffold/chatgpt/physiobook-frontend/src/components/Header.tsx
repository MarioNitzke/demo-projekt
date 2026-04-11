import { Link, NavLink } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

function linkClassName(isActive: boolean) {
  return isActive
    ? "rounded-lg bg-slate-900 px-3 py-2 text-sm font-medium text-white"
    : "rounded-lg px-3 py-2 text-sm font-medium text-slate-600 hover:bg-slate-200";
}

export default function Header() {
  const { isAuthenticated, logout, user } = useAuth();

  return (
    <header className="border-b border-slate-200 bg-white">
      <div className="mx-auto flex max-w-6xl items-center justify-between px-4 py-4">
        <Link to="/" className="text-xl font-bold text-slate-900">
          PhysioBook
        </Link>

        <nav className="flex items-center gap-2">
          <NavLink to="/" className={({ isActive }) => linkClassName(isActive)}>
            Home
          </NavLink>
          <NavLink to="/articles" className={({ isActive }) => linkClassName(isActive)}>
            Articles
          </NavLink>
          {!isAuthenticated ? (
            <>
              <NavLink to="/login" className={({ isActive }) => linkClassName(isActive)}>
                Login
              </NavLink>
              <NavLink to="/register" className={({ isActive }) => linkClassName(isActive)}>
                Register
              </NavLink>
            </>
          ) : (
            <>
              <span className="hidden text-sm text-slate-500 md:inline">{user?.fullName}</span>
              <button
                onClick={logout}
                className="rounded-lg bg-slate-900 px-3 py-2 text-sm font-medium text-white"
              >
                Logout
              </button>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}
