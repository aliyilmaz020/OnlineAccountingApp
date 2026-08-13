import { useState } from "react";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Checkbox from "../../components/form/input/Checkbox";
import Button from "../../components/ui/button/Button";
import type { CrudFormRenderProps } from "../../components/crud/CrudPage";
import type { Role, CreateRoleRequest, UpdateRoleRequest } from "../../types/entities";

type Props = CrudFormRenderProps<Role, CreateRoleRequest, UpdateRoleRequest>;

export default function RoleForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
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
        <Label>Rol Adi</Label>
        <Input value={name} onChange={(e) => setName(e.target.value)} />
      </div>
      <div>
        <Label>Kod</Label>
        <Input value={code} onChange={(e) => setCode(e.target.value)} />
      </div>
      {initial && (
        <div className="flex items-center gap-3">
          <Checkbox checked={status} onChange={setStatus} />
          <span className="text-sm text-gray-700 dark:text-gray-300">Aktif</span>
        </div>
      )}
      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Vazgec
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Kaydediliyor..." : "Kaydet"}
        </Button>
      </div>
    </form>
  );
}
