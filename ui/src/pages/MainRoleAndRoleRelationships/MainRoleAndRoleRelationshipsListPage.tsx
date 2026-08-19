import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import { apiGet } from "../../lib/apiClient";
import type { PagedResult } from "../../types/api";
import type {
  MainRoleAndRoleRelationship,
  CreateMainRoleAndRoleRelationshipRequest,
  UpdateMainRoleAndRoleRelationshipRequest,
  Role,
  MainRole,
} from "../../types/entities";
import MainRoleAndRoleRelationshipForm from "./MainRoleAndRoleRelationshipForm";

export default function MainRoleAndRoleRelationshipsListPage() {
  const { t } = useTranslation(["mainRoleRoleRelationships", "common"]);
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

  const [roles, setRoles] = useState<Role[]>([]);
  const [mainRoles, setMainRoles] = useState<MainRole[]>([]);

  useEffect(() => {
    apiGet<PagedResult<Role>>("/api/Roles/GetRoles", { pageNumber: 1, pageSize: 500 }).then((r) => setRoles(r.items));
    apiGet<PagedResult<MainRole>>("/api/MainRoles/GetMainRoles", { pageNumber: 1, pageSize: 500 }).then((r) =>
      setMainRoles(r.items),
    );
  }, []);

  const roleName = (id: string) => roles.find((r) => r.id === id)?.name ?? id;
  const mainRoleTitle = (id: string) => mainRoles.find((r) => r.id === id)?.title ?? id;

  return (
    <div>
      <PageMeta title={t("mainRoleRoleRelationships:title")} description={t("mainRoleRoleRelationships:description")} />
      <PageBreadcrumb pageTitle={t("mainRoleRoleRelationships:title")} />
      <CrudPage
        title={t("mainRoleRoleRelationships:title")}
        crud={crud}
        columns={[
          { header: t("mainRoleRoleRelationships:columns.roleId"), render: (r) => roleName(r.roleId) },
          { header: t("mainRoleRoleRelationships:columns.mainRoleId"), render: (r) => mainRoleTitle(r.mainRoleId) },
        ]}
        renderForm={(props) => <MainRoleAndRoleRelationshipForm {...props} />}
      />
    </div>
  );
}
