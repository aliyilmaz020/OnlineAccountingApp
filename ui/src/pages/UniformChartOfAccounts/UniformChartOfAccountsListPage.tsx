import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import { usePermission } from "../../context/PermissionContext";
import { UCAF_PERMISSIONS } from "../../constants/permissions";
import type {
  UniformChartOfAccount,
  CreateUniformChartOfAccountRequest,
  UpdateUniformChartOfAccountRequest,
} from "../../types/entities";
import UniformChartOfAccountForm from "./UniformChartOfAccountForm";

export default function UniformChartOfAccountsListPage() {
  const { t } = useTranslation("uniformChartOfAccounts");
  const { hasPermission } = usePermission();
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
      <PageMeta title={t("title")} description={t("description")} />
      <PageBreadcrumb pageTitle={t("title")} />
      <CrudPage
        title={t("title")}
        crud={crud}
        canCreate={hasPermission(UCAF_PERMISSIONS.Create)}
        canEdit={hasPermission(UCAF_PERMISSIONS.Update)}
        canDelete={hasPermission(UCAF_PERMISSIONS.Delete)}
        columns={[
          { header: t("columns.code"), render: (a) => a.code },
          { header: t("columns.name"), render: (a) => a.name },
          { header: t("columns.type"), render: (a) => a.type },
        ]}
        renderForm={(props) => <UniformChartOfAccountForm {...props} />}
      />
    </div>
  );
}
