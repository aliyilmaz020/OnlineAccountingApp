import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import type {
  MainRoleAndRoleRelationship,
  CreateMainRoleAndRoleRelationshipRequest,
  UpdateMainRoleAndRoleRelationshipRequest,
} from "../../types/entities";
import MainRoleAndRoleRelationshipForm from "./MainRoleAndRoleRelationshipForm";

export default function MainRoleAndRoleRelationshipsListPage() {
  const crud = useCrud<
    MainRoleAndRoleRelationship,
    CreateMainRoleAndRoleRelationshipRequest,
    UpdateMainRoleAndRoleRelationshipRequest
  >({
    basePath: "/api/MainRoleAndRoleRelationships",
    getListAction: "GetMainRoleAndRoleRelationships",
    createAction: "CreateMainRoleAndRoleRelationship",
    updateAction: "UpdateMainRoleAndRoleRelationship",
    deleteAction: "DeleteMainRoleAndRoleRelationship",
  });

  return (
    <div>
      <PageMeta title="Ana Rol - Rol Iliskileri" description="Ana rol ve rol iliski yonetimi" />
      <PageBreadcrumb pageTitle="Ana Rol - Rol Iliskileri" />
      <CrudPage
        title="Ana Rol - Rol Iliskileri"
        crud={crud}
        columns={[
          { header: "Rol Id", render: (r) => r.roleId },
          { header: "Ana Rol Id", render: (r) => r.mainRoleId },
        ]}
        renderForm={(props) => <MainRoleAndRoleRelationshipForm {...props} />}
      />
    </div>
  );
}
