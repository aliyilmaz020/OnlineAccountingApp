import type React from "react";
import { Navigate } from "react-router";
import { useCompany } from "../context/CompanyContext";

export default function RequireCompany({ children }: { children: React.ReactNode }) {
  const { selectedCompanyId, isLoadingCompanies } = useCompany();

  if (isLoadingCompanies) {
    return null;
  }

  if (!selectedCompanyId) {
    return <Navigate to="/select-company" replace />;
  }

  return <>{children}</>;
}
