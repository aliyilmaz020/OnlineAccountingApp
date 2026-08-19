import { useState } from "react";
import { useTranslation } from "react-i18next";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Button from "../../components/ui/button/Button";
import type { CrudFormRenderProps } from "../../components/crud/CrudPage";
import type {
  UniformChartOfAccount,
  CreateUniformChartOfAccountRequest,
  UpdateUniformChartOfAccountRequest,
} from "../../types/entities";

type Props = CrudFormRenderProps<
  UniformChartOfAccount,
  CreateUniformChartOfAccountRequest,
  UpdateUniformChartOfAccountRequest
>;

export default function UniformChartOfAccountForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
  const { t } = useTranslation(["uniformChartOfAccounts", "common"]);
  const [code, setCode] = useState(initial?.code ?? "");
  const [name, setName] = useState(initial?.name ?? "");
  const [type, setType] = useState(initial?.type ?? "");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit({ code, name, type });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <Label>{t("uniformChartOfAccounts:form.code")}</Label>
        <Input value={code} onChange={(e) => setCode(e.target.value)} />
      </div>
      <div>
        <Label>{t("uniformChartOfAccounts:form.name")}</Label>
        <Input value={name} onChange={(e) => setName(e.target.value)} />
      </div>
      <div>
        <Label>{t("uniformChartOfAccounts:form.type")}</Label>
        <Input value={type} onChange={(e) => setType(e.target.value)} />
      </div>
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
