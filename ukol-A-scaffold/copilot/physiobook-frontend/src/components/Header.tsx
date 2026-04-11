import { Link, NavLink } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

export function Header() {
  const { isAuthenticated, logout } = useAuth();

  return (
    <header className="border-b bg-white">
      <div className="mx-auto flex max-w-4xl items-center justify-between p-4">
        <Link to="/articles" className="text-lg font-semibold text-slate-900">
          PhysioBook
        </Link>
        <nav className="flex items-center gap-4 text-sm">
          <NavLink to="/articles" className="text-slate-700 hover:text-slate-900">
            Articles
          </NavLink>
          {isAuthenticated ? (
            <>
              <NavLink to="/articles/new" className="text-slate-700 hover:text-slate-900">
                New
              </NavLink>
              <button type="button" onClick={logout} className="rounded bg-slate-900 px-3 py-1 text-white">
                Logout
              </button>
            </>
          ) : (
            <>
              <NavLink to="/login" className="text-slate-700 hover:text-slate-900">
                Login
              </NavLink>
              <NavLink to="/register" className="rounded bg-slate-900 px-3 py-1 text-white">
                Register
              </NavLink>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}

