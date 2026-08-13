import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import { useCompany } from "../../context/CompanyContext";
import type { MainRole, CreateMainRoleRequest, UpdateMainRoleRequest } from "../../types/entities";
import MainRoleForm from "./MainRoleForm";

export default function MainRolesListPage() {
  const { companies } = useCompany();
  const crud = useCrud<MainRole, CreateMainRoleRequest, UpdateMainRoleRequest>({
    basePath: "/api/MainRoles",
    getListAction: "GetMainRoles",
    createAction: "CreateMainRole",
    updateAction: "UpdateMainRole",
    deleteAction: "DeleteMainRole",
  });

  const companyName = (id: string) => companies.find((c) => c.id === id)?.name ?? id;

  return (
    <div>
      <PageMeta title="Ana Roller" description="Ana rol yonetimi" />
      <PageBreadcrumb pageTitle="Ana Roller" />
      <CrudPage
        title="Ana Roller"
        crud={crud}
        columns={[
          { header: "Baslik", render: (r) => r.title },
          { header: "Sirket", render: (r) => companyName(r.companyId) },
          { header: "Admin Olusturabilir", render: (r) => (r.isRoleCreateByAdmin ? "Evet" : "Hayir") },
        ]}
        renderForm={(props) => <MainRoleForm {...props} />}
      />
    </div>
  );
}
