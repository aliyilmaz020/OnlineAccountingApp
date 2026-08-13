import { useState } from "react";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Select from "../../components/form/Select";
import Checkbox from "../../components/form/input/Checkbox";
import Button from "../../components/ui/button/Button";
import { useCompany } from "../../context/CompanyContext";
import type { CrudFormRenderProps } from "../../components/crud/CrudPage";
import type { MainRole, CreateMainRoleRequest, UpdateMainRoleRequest } from "../../types/entities";

type Props = CrudFormRenderProps<MainRole, CreateMainRoleRequest, UpdateMainRoleRequest>;

export default function MainRoleForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
  const { companies } = useCompany();
  const [title, setTitle] = useState(initial?.title ?? "");
  const [companyId, setCompanyId] = useState(initial?.companyId ?? "");
  const [isRoleCreateByAdmin, setIsRoleCreateByAdmin] = useState(initial?.isRoleCreateByAdmin ?? false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit({ title, companyId, isRoleCreateByAdmin });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <Label>Baslik</Label>
        <Input value={title} onChange={(e) => setTitle(e.target.value)} />
      </div>
      <div>
        <Label>Sirket</Label>
        <Select
          options={companies.map((c) => ({ value: c.id, label: c.name }))}
          defaultValue={companyId}
          onChange={setCompanyId}
          placeholder="Sirket secin"
        />
      </div>
      <div className="flex items-center gap-3">
        <Checkbox checked={isRoleCreateByAdmin} onChange={setIsRoleCreateByAdmin} />
        <span className="text-sm text-gray-700 dark:text-gray-300">Admin tarafindan olusturulabilir</span>
      </div>
      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Vazgec
        </Button>
        <Button type="submit" disabled={isSubmitting || !companyId}>
          {isSubmitting ? "Kaydediliyor..." : "Kaydet"}
        </Button>
      </div>
    </form>
  );
}
