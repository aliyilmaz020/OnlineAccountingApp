import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import type { Company, CreateCompanyRequest, UpdateCompanyRequest } from "../../types/entities";
import CompanyForm from "./CompanyForm";

export default function CompaniesListPage() {
  const { t } = useTranslation("companies");
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
        columns={[
          { header: t("columns.name"), render: (c) => c.name },
          { header: t("columns.email"), render: (c) => c.email },
          { header: t("columns.phone"), render: (c) => c.phoneNumber },
          { header: t("columns.taxDepartment"), render: (c) => c.taxDepartment },
        ]}
        renderForm={(props) => <CompanyForm {...props} />}
      />
    </div>
  );
}
