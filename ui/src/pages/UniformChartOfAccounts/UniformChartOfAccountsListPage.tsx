import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import type {
  UniformChartOfAccount,
  CreateUniformChartOfAccountRequest,
  UpdateUniformChartOfAccountRequest,
} from "../../types/entities";
import UniformChartOfAccountForm from "./UniformChartOfAccountForm";

export default function UniformChartOfAccountsListPage() {
  const crud = useCrud<
    UniformChartOfAccount,
    CreateUniformChartOfAccountRequest,
    UpdateUniformChartOfAccountRequest
  >({
    basePath: "/api/UniformChartOfAccounts",
    getListAction: "GetUniformChartOfAccounts",
    createAction: "CreateUniformChartOfAccount",
    updateAction: "UpdateUniformChartOfAccount",
    deleteAction: "DeleteUniformChartOfAccount",
  });

  return (
    <div>
      <PageMeta title="Tekduzen Hesap Plani" description="Tekduzen hesap plani yonetimi" />
      <PageBreadcrumb pageTitle="Tekduzen Hesap Plani" />
      <CrudPage
        title="Tekduzen Hesap Plani"
        crud={crud}
        columns={[
          { header: "Kod", render: (a) => a.code },
          { header: "Ad", render: (a) => a.name },
          { header: "Tip", render: (a) => a.type },
        ]}
        renderForm={(props) => <UniformChartOfAccountForm {...props} />}
      />
    </div>
  );
}
