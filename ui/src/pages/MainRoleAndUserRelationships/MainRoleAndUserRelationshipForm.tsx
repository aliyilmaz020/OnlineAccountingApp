import { useState } from "react";
import { useTranslation } from "react-i18next";
import Label from "../../components/form/Label";
import Select from "../../components/form/Select";
import Button from "../../components/ui/button/Button";
import { useLookupOptions } from "../../hooks/useLookupOptions";
import { useCompany } from "../../context/CompanyContext";
import type { CrudFormRenderProps } from "../../components/crud/CrudPage";
import type {
  MainRoleAndUserRelationship,
  CreateMainRoleAndUserRelationshipRequest,
  UpdateMainRoleAndUserRelationshipRequest,
  MainRole,
  UserListItem,
} from "../../types/entities";

type Props = CrudFormRenderProps<
  MainRoleAndUserRelationship,
  CreateMainRoleAndUserRelationshipRequest,
  UpdateMainRoleAndUserRelationshipRequest
>;

export default function MainRoleAndUserRelationshipForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
  const { t } = useTranslation(["mainRoleUserRelationships", "common"]);
  const { companies } = useCompany();
  const { options: userOptions } = useLookupOptions<UserListItem>(
    "/api/Users",
    "GetUsers",
    (u) => u.userName ?? u.email ?? u.id,
  );
  const { options: mainRoleOptions } = useLookupOptions<MainRole>("/api/MainRoles", "GetMainRoles", (r) => r.title);

  const [userId, setUserId] = useState(initial?.userId ?? "");
  const [mainRoleId, setMainRoleId] = useState(initial?.mainRoleId ?? "");
  const [companyId, setCompanyId] = useState(initial?.companyId ?? "");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit({ userId, mainRoleId, companyId });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <Label>{t("mainRoleUserRelationships:form.userLabel")}</Label>
        <Select
          options={userOptions}
          defaultValue={userId}
          onChange={setUserId}
          placeholder={t("mainRoleUserRelationships:form.userPlaceholder")}
        />
      </div>
      <div>
        <Label>{t("mainRoleUserRelationships:form.mainRoleLabel")}</Label>
        <Select
          options={mainRoleOptions}
          defaultValue={mainRoleId}
          onChange={setMainRoleId}
          placeholder={t("mainRoleUserRelationships:form.mainRolePlaceholder")}
        />
      </div>
      <div>
        <Label>{t("mainRoleUserRelationships:form.companyLabel")}</Label>
        <Select
          options={companies.map((c) => ({ value: c.id, label: c.name }))}
          defaultValue={companyId}
          onChange={setCompanyId}
          placeholder={t("mainRoleUserRelationships:form.companyPlaceholder")}
        />
      </div>
      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          {t("common:cancel")}
        </Button>
        <Button type="submit" disabled={isSubmitting || !userId || !mainRoleId || !companyId}>
          {isSubmitting ? t("common:saving") : t("common:save")}
        </Button>
      </div>
    </form>
  );
}
