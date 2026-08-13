import { useState } from "react";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Button from "../../components/ui/button/Button";
import Alert from "../../components/ui/alert/Alert";
import type { CrudFormRenderProps } from "../../components/crud/CrudPage";
import type { Company, CreateCompanyRequest, UpdateCompanyRequest } from "../../types/entities";

type Props = CrudFormRenderProps<Company, CreateCompanyRequest, UpdateCompanyRequest>;

export default function CompanyForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
  const [name, setName] = useState(initial?.name ?? "");
  const [address, setAddress] = useState(initial?.address ?? "");
  const [identityNumber, setIdentityNumber] = useState(initial?.identityNumber ?? "");
  const [taxDepartment, setTaxDepartment] = useState(initial?.taxDepartment ?? "");
  const [phoneNumber, setPhoneNumber] = useState(initial?.phoneNumber ?? "");
  const [email, setEmail] = useState(initial?.email ?? "");
  const [serverName, setServerName] = useState("");
  const [databaseName, setDatabaseName] = useState("");
  const [serverUserId, setServerUserId] = useState("");
  const [serverPassword, setServerPassword] = useState("");

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    await onSubmit({
      name,
      address,
      identityNumber,
      taxDepartment,
      phoneNumber,
      email,
      serverName,
      databaseName,
      serverUserId,
      serverPassword,
    });
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      {initial && (
        <Alert
          variant="warning"
          title="Dikkat"
          message="Sunucu bilgileri (Server/Database/User/Password) guvenlik nedeniyle geri gosterilmez. Bu alanlari bos birakirsaniz mevcut baglanti bilgileri silinir - degistirmek istemiyorsaniz ayni degerleri tekrar girin."
        />
      )}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <Label>Sirket Adi</Label>
          <Input value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div>
          <Label>Adres</Label>
          <Input value={address} onChange={(e) => setAddress(e.target.value)} />
        </div>
        <div>
          <Label>Vergi No / Kimlik No</Label>
          <Input value={identityNumber} onChange={(e) => setIdentityNumber(e.target.value)} />
        </div>
        <div>
          <Label>Vergi Dairesi</Label>
          <Input value={taxDepartment} onChange={(e) => setTaxDepartment(e.target.value)} />
        </div>
        <div>
          <Label>Telefon</Label>
          <Input value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} />
        </div>
        <div>
          <Label>E-posta</Label>
          <Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
      </div>

      <div className="border-t border-gray-200 pt-4 dark:border-gray-800">
        <h5 className="mb-3 text-sm font-medium text-gray-700 dark:text-gray-300">
          Veritabani Baglanti Bilgileri
        </h5>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <Label>Sunucu Adi</Label>
            <Input value={serverName} onChange={(e) => setServerName(e.target.value)} />
          </div>
          <div>
            <Label>Veritabani Adi</Label>
            <Input value={databaseName} onChange={(e) => setDatabaseName(e.target.value)} />
          </div>
          <div>
            <Label>Sunucu Kullanici Adi</Label>
            <Input value={serverUserId} onChange={(e) => setServerUserId(e.target.value)} />
          </div>
          <div>
            <Label>Sunucu Sifresi</Label>
            <Input
              type="password"
              value={serverPassword}
              onChange={(e) => setServerPassword(e.target.value)}
            />
          </div>
        </div>
      </div>

      <div className="flex justify-end gap-3 pt-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Vazgec
        </Button>
        <Button type="submit" disabled={isSubmitting}>{isSubmitting ? "Kaydediliyor..." : "Kaydet"}</Button>
      </div>
    </form>
  );
}
