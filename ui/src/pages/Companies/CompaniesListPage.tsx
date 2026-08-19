import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import { useAuth } from "../../context/AuthContext";
import { usePermission } from "../../context/PermissionContext";
import { COMPANY_PERMISSIONS } from "../../constants/permissions";
import type { Company, CreateCompanyRequest, UpdateCompanyRequest } from "../../types/entities";
import CompanyForm from "./CompanyForm";

export default function CompaniesListPage() {
  const { t } = useTranslation("companies");
  const { user } = useAuth();
  const { hasPermission } = usePermission();
  const isAdmin = user?.isAdmin ?? false;
  // Editing (minus DB fields, enforced server-side) is also open to a company's own "Yönetici"
  // via Company.Update; creating/deleting a company stays system-admin-only.
  const canEdit = isAdmin || hasPermission(COMPANY_PERMISSIONS.Update);
  const crud = useCrud<Company, CreateCompanyRequest, UpdateCompanyRequest>({
    basePath: "/api/Companies",
    getListAction: "GetCompanies",
    createAction: "CreateCompany",
    updateAction: "UpdateCompany",
    deleteAction: "DeleteCompany",
  });

  return (
    <div>
      <PageMeta title={t("title")} description={t("description")} />
      <PageBreadcrumb pageTitle={t("title")} />
      <CrudPage
        title={t("title")}
        crud={crud}
        canCreate={isAdmin}
        canEdit={canEdit}
        canDelete={isAdmin}
        columns={[
          { header: t("columns.name"), render: (c) => c.name },
          { header: t("columns.email"), render: (c) => c.email },
          { header: t("columns.phone"), render: (c) => c.phoneNumber },
          { header: t("columns.taxDepartment"), render: (c) => c.taxDepartment },
        ]}
        renderForm={(props) => <CompanyForm {...props} isAdmin={isAdmin} />}
      />
    </div>
  );
}
