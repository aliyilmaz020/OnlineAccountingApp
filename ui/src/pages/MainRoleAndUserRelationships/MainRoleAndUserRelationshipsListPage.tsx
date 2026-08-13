import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import type {
  MainRoleAndUserRelationship,
  CreateMainRoleAndUserRelationshipRequest,
  UpdateMainRoleAndUserRelationshipRequest,
} from "../../types/entities";
import MainRoleAndUserRelationshipForm from "./MainRoleAndUserRelationshipForm";

export default function MainRoleAndUserRelationshipsListPage() {
  const crud = useCrud<
    MainRoleAndUserRelationship,
    CreateMainRoleAndUserRelationshipRequest,
    UpdateMainRoleAndUserRelationshipRequest
  >({
    basePath: "/api/MainRoleAndUserRelationships",
    getListAction: "GetMainRoleAndUserRelationships",
    createAction: "CreateMainRoleAndUserRelationship",
    updateAction: "UpdateMainRoleAndUserRelationship",
    deleteAction: "DeleteMainRoleAndUserRelationship",
  });

  return (
    <div>
      <PageMeta title="Ana Rol - Kullanici Iliskileri" description="Ana rol ve kullanici iliski yonetimi" />
      <PageBreadcrumb pageTitle="Ana Rol - Kullanici Iliskileri" />
      <CrudPage
        title="Ana Rol - Kullanici Iliskileri"
        crud={crud}
        columns={[
          { header: "Kullanici Id", render: (r) => r.userId },
          { header: "Ana Rol Id", render: (r) => r.mainRoleId },
          { header: "Sirket Id", render: (r) => r.companyId },
        ]}
        renderForm={(props) => <MainRoleAndUserRelationshipForm {...props} />}
      />
    </div>
  );
}
