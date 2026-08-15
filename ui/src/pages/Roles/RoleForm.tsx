import { useState } from "react";
import { useTranslation } from "react-i18next";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Checkbox from "../../components/form/input/Checkbox";
import Button from "../../components/ui/button/Button";
import type { CrudFormRenderProps } from "../../components/crud/CrudPage";
import type { Role, CreateRoleRequest, UpdateRoleRequest } from "../../types/entities";

type Props = CrudFormRenderProps<Role, CreateRoleRequest, UpdateRoleRequest>;

export default function RoleForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
  const { t } = useTranslation(["roles", "common"]);
  const [name, setName] = useState(initial?.name ?? "");
  const [code, setCode] = useState(initial?.code ?? "");
  const [status, setStatus] = useState(initial?.status ?? true);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit({ name, code, status });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <Label>{t("roles:form.roleName")}</Label>
        <Input value={name} onChange={(e) => setName(e.target.value)} />
      </div>
      <div>
        <Label>{t("roles:form.code")}</Label>
        <Input value={code} onChange={(e) => setCode(e.target.value)} />
      </div>
      {initial && (
        <div className="flex items-center gap-3">
          <Checkbox checked={status} onChange={setStatus} />
          <span className="text-sm text-gray-700 dark:text-gray-300">{t("roles:form.active")}</span>
        </div>
      )}
      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          {t("common:cancel")}
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? t("common:saving") : t("common:save")}
        </Button>
      </div>
    </form>
  );
}
