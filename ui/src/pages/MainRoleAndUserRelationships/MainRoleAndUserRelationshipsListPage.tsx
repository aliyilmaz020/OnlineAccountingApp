import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import CrudPage from "../../components/crud/CrudPage";
import { useCrud } from "../../hooks/useCrud";
import { useCompany } from "../../context/CompanyContext";
import { apiGet } from "../../lib/apiClient";
import type { PagedResult } from "../../types/api";
import type {
  MainRoleAndUserRelationship,
  CreateMainRoleAndUserRelationshipRequest,
  UpdateMainRoleAndUserRelationshipRequest,
  MainRole,
  UserListItem,
} from "../../types/entities";
import MainRoleAndUserRelationshipForm from "./MainRoleAndUserRelationshipForm";

export default function MainRoleAndUserRelationshipsListPage() {
  const { t } = useTranslation(["mainRoleUserRelationships", "common"]);
  const { companies } = useCompany();
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

  const [mainRoles, setMainRoles] = useState<MainRole[]>([]);
  const [users, setUsers] = useState<UserListItem[]>([]);

  useEffect(() => {
    apiGet<PagedResult<MainRole>>("/api/MainRoles/GetMainRoles", { pageNumber: 1, pageSize: 500 }).then((r) =>
      setMainRoles(r.items),
    );
    apiGet<PagedResult<UserListItem>>("/api/Users/GetUsers", { pageNumber: 1, pageSize: 500 }).then((r) =>
      setUsers(r.items),
    );
  }, []);

  const mainRoleTitle = (id: string) => mainRoles.find((r) => r.id === id)?.title ?? id;
  const userName = (id: string) => {
    const user = users.find((u) => u.id === id);
    return user?.email ?? user?.userName ?? id;
  };
  const companyName = (id: string) => companies.find((c) => c.id === id)?.name ?? id;

  return (
    <div>
      <PageMeta title={t("mainRoleUserRelationships:title")} description={t("mainRoleUserRelationships:description")} />
      <PageBreadcrumb pageTitle={t("mainRoleUserRelationships:title")} />
      <CrudPage
        title={t("mainRoleUserRelationships:title")}
        crud={crud}
        columns={[
          { header: t("mainRoleUserRelationships:columns.userId"), render: (r) => userName(r.userId) },
          { header: t("mainRoleUserRelationships:columns.mainRoleId"), render: (r) => mainRoleTitle(r.mainRoleId) },
          { header: t("mainRoleUserRelationships:columns.companyId"), render: (r) => companyName(r.companyId) },
        ]}
        renderForm={(props) => <MainRoleAndUserRelationshipForm {...props} />}
      />
    </div>
  );
}
