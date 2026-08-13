import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import type { Company, CreateCompanyRequest, UpdateCompanyRequest } from "../../types/entities";
import CompanyForm from "./CompanyForm";

export default function CompaniesListPage() {
  const crud = useCrud<Company, CreateCompanyRequest, UpdateCompanyRequest>({
    basePath: "/api/Companies",
    getListAction: "GetCompanies",
    createAction: "CreateCompany",
    updateAction: "UpdateCompany",
    deleteAction: "DeleteCompany",
  });

  return (
    <div>
      <PageMeta title="Sirketler" description="Sirket yonetimi" />
      <PageBreadcrumb pageTitle="Sirketler" />
      <CrudPage
        title="Sirketler"
        crud={crud}
        columns={[
          { header: "Ad", render: (c) => c.name },
          { header: "E-posta", render: (c) => c.email },
          { header: "Telefon", render: (c) => c.phoneNumber },
          { header: "Vergi Dairesi", render: (c) => c.taxDepartment },
        ]}
        renderForm={(props) => <CompanyForm {...props} />}
      />
    </div>
  );
}
