import { useState } from "react";
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
        <Label>Rol</Label>
        <Select options={roleOptions} defaultValue={roleId} onChange={setRoleId} placeholder="Rol secin" />
      </div>
      <div>
        <Label>Ana Rol</Label>
        <Select
          options={mainRoleOptions}
          defaultValue={mainRoleId}
          onChange={setMainRoleId}
          placeholder="Ana rol secin"
        />
      </div>
      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Vazgec
        </Button>
        <Button type="submit" disabled={isSubmitting || !roleId || !mainRoleId}>
          {isSubmitting ? "Kaydediliyor..." : "Kaydet"}
        </Button>
      </div>
    </form>
  );
}
