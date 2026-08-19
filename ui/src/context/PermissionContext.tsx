import type React from "react";
import { createContext, useState, useContext, useEffect, useCallback } from "react";
import { apiGet } from "../lib/apiClient";
import { useAuth } from "./AuthContext";
import { useCompany } from "./CompanyContext";

type PermissionContextType = {
  permissions: string[];
  isLoadingPermissions: boolean;
  hasPermission: (code: string) => boolean;
};

const PermissionContext = createContext<PermissionContextType | undefined>(undefined);

export const PermissionProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const { isAuthenticated } = useAuth();
  const { selectedCompanyId } = useCompany();
  const [permissions, setPermissions] = useState<string[]>([]);
  const [isLoadingPermissions, setIsLoadingPermissions] = useState(false);

  useEffect(() => {
    if (!isAuthenticated || !selectedCompanyId) {
      setPermissions([]);
      return;
    }

    let cancelled = false;
    setIsLoadingPermissions(true);
    apiGet<string[]>("/api/Permissions/GetMyPermissions")
      .then((codes) => {
        if (!cancelled) setPermissions(codes);
      })
      .catch(() => {
        if (!cancelled) setPermissions([]);
      })
      .finally(() => {
        if (!cancelled) setIsLoadingPermissions(false);
      });

    return () => {
      cancelled = true;
    };
  }, [isAuthenticated, selectedCompanyId]);

  const hasPermission = useCallback((code: string) => permissions.includes(code), [permissions]);

  return (
    <PermissionContext.Provider value={{ permissions, isLoadingPermissions, hasPermission }}>
      {children}
    </PermissionContext.Provider>
  );
};

export const usePermission = () => {
  const context = useContext(PermissionContext);
  if (context === undefined) {
    throw new Error("usePermission must be used within a PermissionProvider");
  }
  return context;
};
