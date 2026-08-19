import type React from "react";
import { createContext, useState, useContext, useEffect, useCallback } from "react";
import { apiGet, apiPost } from "../lib/apiClient";
import {
  getAccessToken,
  clearTokens,
  clearSelectedCompanyId,
  setTokens,
  setUnauthorizedHandler,
} from "../lib/apiClient";
import type { AuthResponse, LoginRequest, RegisterRequest } from "../types/auth";
import type { MyProfile } from "../types/entities";

type AuthContextType = {
  isAuthenticated: boolean;
  isInitializing: boolean;
  user: MyProfile | null;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
  logout: () => void;
  refreshUser: () => Promise<void>;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isInitializing, setIsInitializing] = useState(true);
  const [user, setUser] = useState<MyProfile | null>(null);

  const logout = useCallback(() => {
    clearTokens();
    clearSelectedCompanyId();
    setIsAuthenticated(false);
    setUser(null);
    window.dispatchEvent(new Event("auth:logout"));
  }, []);

  const refreshUser = useCallback(async () => {
    try {
      const profile = await apiGet<MyProfile>("/api/Users/GetMyProfile");
      setUser(profile);
    } catch {
      setUser(null);
    }
  }, []);

  useEffect(() => {
    const hasToken = !!getAccessToken();
    setIsAuthenticated(hasToken);
    setIsInitializing(false);
    setUnauthorizedHandler(logout);
    if (hasToken) {
      refreshUser();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [logout]);

  const login = useCallback(async (email: string, password: string) => {
    const auth = await apiPost<AuthResponse>("/api/Auth/Login", { email, password } satisfies LoginRequest);
    setTokens(auth);
    setIsAuthenticated(true);
    await refreshUser();
  }, [refreshUser]);

  const register = useCallback(async (email: string, password: string) => {
    const auth = await apiPost<AuthResponse>("/api/Auth/Register", { email, password } satisfies RegisterRequest);
    setTokens(auth);
    setIsAuthenticated(true);
    await refreshUser();
  }, [refreshUser]);

  return (
    <AuthContext.Provider value={{ isAuthenticated, isInitializing, user, login, register, logout, refreshUser }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
