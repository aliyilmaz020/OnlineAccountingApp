import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import type { Role, CreateRoleRequest, UpdateRoleRequest } from "../../types/entities";
import RoleForm from "./RoleForm";

export default function RolesListPage() {
  const { t } = useTranslation(["roles", "common"]);
  const crud = useCrud<Role, CreateRoleRequest, UpdateRoleRequest>({
    basePath: "/api/Roles",
    getListAction: "GetRoles",
    createAction: "CreateRole",
    updateAction: "UpdateRole",
    deleteAction: "DeleteRole",
  });

  return (
    <div>
      <PageMeta title={t("roles:title")} description={t("roles:description")} />
      <PageBreadcrumb pageTitle={t("roles:title")} />
      <CrudPage
        title={t("roles:title")}
        crud={crud}
        columns={[
          { header: t("roles:columns.name"), render: (r) => r.name },
          { header: t("roles:columns.code"), render: (r) => r.code },
          { header: t("roles:columns.status"), render: (r) => (r.status ? t("common:active") : t("common:inactive")) },
        ]}
        renderForm={(props) => <RoleForm {...props} />}
      />
    </div>
  );
}
