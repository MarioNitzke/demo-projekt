import { createContext, useContext, useMemo, useState } from 'react';
import { BaseApiService } from '../shared/BaseApiService';

type AuthState = {
  accessToken: string | null;
  refreshToken: string | null;
  email: string | null;
};

type LoginPayload = { email: string; password: string };
type RegisterPayload = { email: string; password: string };

type AuthContextValue = {
  auth: AuthState;
  isAuthenticated: boolean;
  login: (payload: LoginPayload) => Promise<void>;
  register: (payload: RegisterPayload) => Promise<void>;
  refresh: () => Promise<void>;
  logout: () => void;
};

const STORAGE_KEY = 'physiobook_auth';
const AuthContext = createContext<AuthContextValue | null>(null);

function loadInitialState(): AuthState {
  const raw = localStorage.getItem(STORAGE_KEY);
  if (!raw) {
    return { accessToken: null, refreshToken: null, email: null };
  }

  try {
    return JSON.parse(raw) as AuthState;
  } catch {
    return { accessToken: null, refreshToken: null, email: null };
  }
}

function saveState(state: AuthState): void {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
}

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [auth, setAuth] = useState<AuthState>(() => loadInitialState());

  const updateAuth = (state: AuthState) => {
    setAuth(state);
    saveState(state);
  };

  const login = async (payload: LoginPayload) => {
    const response = await BaseApiService.post<{ accessToken: string; refreshToken: string; expiresAtUtc: string }>('/auth/login', payload);
    updateAuth({ accessToken: response.accessToken, refreshToken: response.refreshToken, email: payload.email });
  };

  const register = async (payload: RegisterPayload) => {
    await BaseApiService.post('/auth/register', payload);
    await login(payload);
  };

  const refresh = async () => {
    if (!auth.refreshToken || !auth.email) {
      throw new Error('No refresh token available.');
    }

    const response = await BaseApiService.post<{ accessToken: string; refreshToken: string; expiresAtUtc: string }>('/auth/refresh-token', {
      email: auth.email,
      refreshToken: auth.refreshToken
    });

    updateAuth({ accessToken: response.accessToken, refreshToken: response.refreshToken, email: auth.email });
  };

  const logout = () => {
    updateAuth({ accessToken: null, refreshToken: null, email: null });
  };

  const value = useMemo<AuthContextValue>(
    () => ({ auth, isAuthenticated: Boolean(auth.accessToken), login, register, refresh, logout }),
    [auth]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider.');
  }

  return context;
}

