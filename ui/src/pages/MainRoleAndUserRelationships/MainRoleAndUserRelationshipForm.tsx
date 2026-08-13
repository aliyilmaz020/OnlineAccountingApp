import { useState } from "react";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
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
} from "../../types/entities";

type Props = CrudFormRenderProps<
  MainRoleAndUserRelationship,
  CreateMainRoleAndUserRelationshipRequest,
  UpdateMainRoleAndUserRelationshipRequest
>;

export default function MainRoleAndUserRelationshipForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
  const { companies } = useCompany();
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
        <Label>Kullanici Id</Label>
        <Input
          value={userId}
          onChange={(e) => setUserId(e.target.value)}
          hint="Bu ekran icin kullanici listeleme endpoint'i bulunmuyor; kullanicinin Id'sini manuel girin."
        />
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
      <div>
        <Label>Sirket</Label>
        <Select
          options={companies.map((c) => ({ value: c.id, label: c.name }))}
          defaultValue={companyId}
          onChange={setCompanyId}
          placeholder="Sirket secin"
        />
      </div>
      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Vazgec
        </Button>
        <Button type="submit" disabled={isSubmitting || !userId || !mainRoleId || !companyId}>
          {isSubmitting ? "Kaydediliyor..." : "Kaydet"}
        </Button>
      </div>
    </form>
  );
}
