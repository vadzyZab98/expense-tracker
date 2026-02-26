import { createContext, useContext, useState, useCallback, useMemo } from 'react';
import type { ReactNode } from 'react';

interface AuthState {
  token: string | null;
  role: string | null;
  email: string | null;
}

interface AuthContextValue extends AuthState {
  isAuthenticated: boolean;
  login: (token: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | null>(null);

const parseToken = (token: string | null): Omit<AuthState, 'token'> => {
  if (!token) return { role: null, email: null };
  try {
    const payload = JSON.parse(atob(token.split('.')[1]));
    return {
      role: payload.role ?? payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? null,
      email: payload.email ?? payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'] ?? null,
    };
  } catch {
    return { role: null, email: null };
  }
};

export function AuthProvider({ children }: { children: ReactNode }) {
  const [authState, setAuthState] = useState<AuthState>(() => {
    const token = localStorage.getItem('token');
    return { token, ...parseToken(token) };
  });

  const login = useCallback((token: string) => {
    localStorage.setItem('token', token);
    setAuthState({ token, ...parseToken(token) });
  }, []);

  const logout = useCallback(() => {
    localStorage.clear();
    setAuthState({ token: null, role: null, email: null });
  }, []);

  const value = useMemo<AuthContextValue>(() => ({
    ...authState,
    isAuthenticated: Boolean(authState.token),
    login,
    logout,
  }), [authState, login, logout]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
}
