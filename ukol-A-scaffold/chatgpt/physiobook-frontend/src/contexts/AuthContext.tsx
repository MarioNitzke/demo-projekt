import { createContext, useContext, useEffect, useMemo, useState } from "react";
import authService from "../features/auth/authService";
import type { AuthResponse, LoginRequest, RegisterRequest } from "../features/auth/authTypes";

interface AuthContextValue {
  user: AuthResponse | null;
  isAuthenticated: boolean;
  login: (request: LoginRequest) => Promise<void>;
  register: (request: RegisterRequest) => Promise<void>;
  refreshToken: () => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

const ACCESS_TOKEN_KEY = "physiobook.accessToken";
const REFRESH_TOKEN_KEY = "physiobook.refreshToken";
const USER_KEY = "physiobook.user";

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<AuthResponse | null>(null);

  useEffect(() => {
    const serializedUser = localStorage.getItem(USER_KEY);
    if (serializedUser) {
      setUser(JSON.parse(serializedUser) as AuthResponse);
    }
  }, []);

  const persist = (auth: AuthResponse) => {
    localStorage.setItem(ACCESS_TOKEN_KEY, auth.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, auth.refreshToken);
    localStorage.setItem(USER_KEY, JSON.stringify(auth));
    setUser(auth);
  };

  const login = async (request: LoginRequest) => {
    const response = await authService.login(request);
    persist(response);
  };

  const register = async (request: RegisterRequest) => {
    const response = await authService.register(request);
    persist(response);
  };

  const refreshToken = async () => {
    const token = localStorage.getItem(REFRESH_TOKEN_KEY);
    if (!token) {
      logout();
      return;
    }

    const response = await authService.refreshToken({ refreshToken: token });
    persist(response);
  };

  const logout = () => {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setUser(null);
  };

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: !!user?.accessToken,
      login,
      register,
      refreshToken,
      logout
    }),
    [user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within AuthProvider.");
  }

  return context;
}
