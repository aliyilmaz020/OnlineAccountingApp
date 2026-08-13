import type React from "react";
import { createContext, useState, useContext, useEffect, useCallback } from "react";
import { apiGet, getSelectedCompanyId, setSelectedCompanyId as persistSelectedCompanyId } from "../lib/apiClient";
import type { PagedResult } from "../types/api";
import type { Company } from "../types/entities";
import { useAuth } from "./AuthContext";

type CompanyContextType = {
  companies: Company[];
  selectedCompanyId: string | null;
  isLoadingCompanies: boolean;
  selectCompany: (id: string) => void;
  refreshCompanies: () => Promise<void>;
};

const CompanyContext = createContext<CompanyContextType | undefined>(undefined);

export const CompanyProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  const [companies, setCompanies] = useState<Company[]>([]);
  const [selectedCompanyId, setSelectedCompanyIdState] = useState<string | null>(getSelectedCompanyId());
  const [isLoadingCompanies, setIsLoadingCompanies] = useState(false);

  const refreshCompanies = useCallback(async () => {
    setIsLoadingCompanies(true);
    try {
      const result = await apiGet<PagedResult<Company>>("/api/Companies/GetCompanies", {
        pageNumber: 1,
        pageSize: 100,
      });
      setCompanies(result.items);
    } finally {
      setIsLoadingCompanies(false);
    }
  }, []);

  useEffect(() => {
    if (isAuthenticated) {
      refreshCompanies();
    } else {
      setCompanies([]);
    }
  }, [isAuthenticated, refreshCompanies]);

  useEffect(() => {
    const handleLogout = () => {
      setCompanies([]);
      setSelectedCompanyIdState(null);
    };
    window.addEventListener("auth:logout", handleLogout);
    return () => window.removeEventListener("auth:logout", handleLogout);
  }, []);

  const selectCompany = useCallback((id: string) => {
    persistSelectedCompanyId(id);
    setSelectedCompanyIdState(id);
  }, []);

  return (
    <CompanyContext.Provider
      value={{ companies, selectedCompanyId, isLoadingCompanies, selectCompany, refreshCompanies }}
    >
      {children}
    </CompanyContext.Provider>
  );
};

export const useCompany = () => {
  const context = useContext(CompanyContext);
  if (context === undefined) {
    throw new Error("useCompany must be used within a CompanyProvider");
  }
  return context;
};
