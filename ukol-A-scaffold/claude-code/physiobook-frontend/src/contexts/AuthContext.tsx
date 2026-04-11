import React, { createContext, useContext, useState, useEffect, useCallback, type ReactNode } from 'react';
import api from '@/shared/BaseApiService';

interface User {
  id: string;
  email: string;
  roles: string[];
}

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string, firstName: string, lastName: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

function decodeJwt(token: string): User | null {
  try {
    const payload = token.split('.')[1];
    const decoded = JSON.parse(atob(payload));
    return {
      id: decoded.sub || decoded.nameid || decoded.id || '',
      email:
        decoded.email ||
        decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ||
        '',
      roles: Array.isArray(decoded.role || decoded.roles)
        ? decoded.role || decoded.roles
        : decoded.role || decoded.roles
          ? [decoded.role || decoded.roles]
          : decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
            ? Array.isArray(decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'])
              ? decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
              : [decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']]
            : [],
    };
  } catch {
    return null;
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    const storedToken = localStorage.getItem('accessToken');
    if (storedToken) {
      const decoded = decodeJwt(storedToken);
      if (decoded) {
        setToken(storedToken);
        setUser(decoded);
      } else {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
      }
    }
  }, []);

  const login = useCallback(async (email: string, password: string) => {
    const response = await api.post('/auth/login', { email, password });
    const { token: accessToken, refreshToken } = response.data;
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    setToken(accessToken);
    const decoded = decodeJwt(accessToken);
    setUser(decoded);
  }, []);

  const register = useCallback(
    async (email: string, password: string, firstName: string, lastName: string) => {
      await api.post('/auth/register', { email, password, firstName, lastName });
    },
    [],
  );

  const logout = useCallback(() => {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    setToken(null);
    setUser(null);
  }, []);

  const isAuthenticated = !!user && !!token;
  const isAdmin = user?.roles.some((r) => r.toLowerCase() === 'admin') ?? false;

  return (
    <AuthContext.Provider
      value={{ user, token, isAuthenticated, isAdmin, login, register, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

export default AuthContext;
