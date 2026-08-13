import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import type { Role, CreateRoleRequest, UpdateRoleRequest } from "../../types/entities";
import RoleForm from "./RoleForm";

export default function RolesListPage() {
  const crud = useCrud<Role, CreateRoleRequest, UpdateRoleRequest>({
    basePath: "/api/Roles",
    getListAction: "GetRoles",
    createAction: "CreateRole",
    updateAction: "UpdateRole",
    deleteAction: "DeleteRole",
  });

  return (
    <div>
      <PageMeta title="Roller" description="Rol yonetimi" />
      <PageBreadcrumb pageTitle="Roller" />
      <CrudPage
        title="Roller"
        crud={crud}
        columns={[
          { header: "Ad", render: (r) => r.name },
          { header: "Kod", render: (r) => r.code },
          { header: "Durum", render: (r) => (r.status ? "Aktif" : "Pasif") },
        ]}
        renderForm={(props) => <RoleForm {...props} />}
      />
    </div>
  );
}
