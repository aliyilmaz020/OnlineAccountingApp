import type React from "react";
import { createContext, useState, useContext, useEffect, useCallback } from "react";
import { apiPost } from "../lib/apiClient";
import {
  getAccessToken,
  clearTokens,
  clearSelectedCompanyId,
  setTokens,
  setUnauthorizedHandler,
} from "../lib/apiClient";
import type { AuthResponse, LoginRequest, RegisterRequest } from "../types/auth";

type AuthContextType = {
  isAuthenticated: boolean;
  isInitializing: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (email: string, password: string) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isInitializing, setIsInitializing] = useState(true);

  const logout = useCallback(() => {
    clearTokens();
    clearSelectedCompanyId();
    setIsAuthenticated(false);
    window.dispatchEvent(new Event("auth:logout"));
  }, []);

  useEffect(() => {
    setIsAuthenticated(!!getAccessToken());
    setIsInitializing(false);
    setUnauthorizedHandler(logout);
  }, [logout]);

  const login = useCallback(async (email: string, password: string) => {
    const auth = await apiPost<AuthResponse>("/api/Auth/Login", { email, password } satisfies LoginRequest);
    setTokens(auth);
    setIsAuthenticated(true);
  }, []);

  const register = useCallback(async (email: string, password: string) => {
    const auth = await apiPost<AuthResponse>("/api/Auth/Register", { email, password } satisfies RegisterRequest);
    setTokens(auth);
    setIsAuthenticated(true);
  }, []);

  return (
    <AuthContext.Provider value={{ isAuthenticated, isInitializing, login, register, logout }}>
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
