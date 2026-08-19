import { useState } from "react";
import { useTranslation } from "react-i18next";
import Label from "../../components/form/Label";
import Select from "../../components/form/Select";
import Button from "../../components/ui/button/Button";
import { useLookupOptions } from "../../hooks/useLookupOptions";
import type { CrudFormRenderProps } from "../../components/crud/CrudPage";
import type {
  MainRoleAndRoleRelationship,
  CreateMainRoleAndRoleRelationshipRequest,
  UpdateMainRoleAndRoleRelationshipRequest,
  Role,
  MainRole,
} from "../../types/entities";

type Props = CrudFormRenderProps<
  MainRoleAndRoleRelationship,
  CreateMainRoleAndRoleRelationshipRequest,
  UpdateMainRoleAndRoleRelationshipRequest
>;

export default function MainRoleAndRoleRelationshipForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
  const { t } = useTranslation(["mainRoleRoleRelationships", "common"]);
  const { options: roleOptions } = useLookupOptions<Role>("/api/Roles", "GetRoles", (r) => `${r.name} (${r.code})`);
  const { options: mainRoleOptions } = useLookupOptions<MainRole>("/api/MainRoles", "GetMainRoles", (r) => r.title);

  const [roleId, setRoleId] = useState(initial?.roleId ?? "");
  const [mainRoleId, setMainRoleId] = useState(initial?.mainRoleId ?? "");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit({ roleId, mainRoleId });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <Label>{t("mainRoleRoleRelationships:form.role")}</Label>
        <Select
          options={roleOptions}
          defaultValue={roleId}
          onChange={setRoleId}
          placeholder={t("mainRoleRoleRelationships:form.rolePlaceholder")}
        />
      </div>
      <div>
        <Label>{t("mainRoleRoleRelationships:form.mainRole")}</Label>
        <Select
          options={mainRoleOptions}
          defaultValue={mainRoleId}
          onChange={setMainRoleId}
          placeholder={t("mainRoleRoleRelationships:form.mainRolePlaceholder")}
        />
      </div>
      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          {t("common:cancel")}
        </Button>
        <Button type="submit" disabled={isSubmitting || !roleId || !mainRoleId}>
          {isSubmitting ? t("common:saving") : t("common:save")}
        </Button>
      </div>
    </form>
  );
}
