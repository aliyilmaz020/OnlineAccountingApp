import { useState } from "react";
import { useTranslation } from "react-i18next";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Button from "../../components/ui/button/Button";
import Alert from "../../components/ui/alert/Alert";
import type { CrudFormRenderProps } from "../../components/crud/CrudPage";
import type { Company, CreateCompanyRequest, UpdateCompanyRequest } from "../../types/entities";

type Props = CrudFormRenderProps<Company, CreateCompanyRequest, UpdateCompanyRequest>;

export default function CompanyForm({ initial, onSubmit, onCancel, isSubmitting }: Props) {
  const { t } = useTranslation(["companies", "common"]);
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
          title={t("companies:form.serverInfoWarningTitle")}
          message={t("companies:form.serverInfoWarning")}
        />
      )}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
        <div>
          <Label>{t("companies:form.companyName")}</Label>
          <Input value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div>
          <Label>{t("companies:form.address")}</Label>
          <Input value={address} onChange={(e) => setAddress(e.target.value)} />
        </div>
        <div>
          <Label>{t("companies:form.identityNumber")}</Label>
          <Input value={identityNumber} onChange={(e) => setIdentityNumber(e.target.value)} />
        </div>
        <div>
          <Label>{t("companies:form.taxDepartment")}</Label>
          <Input value={taxDepartment} onChange={(e) => setTaxDepartment(e.target.value)} />
        </div>
        <div>
          <Label>{t("companies:form.phoneNumber")}</Label>
          <Input value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} />
        </div>
        <div>
          <Label>{t("companies:form.email")}</Label>
          <Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
      </div>

      <div className="border-t border-gray-200 pt-4 dark:border-gray-800">
        <h5 className="mb-3 text-sm font-medium text-gray-700 dark:text-gray-300">
          {t("companies:form.dbSectionTitle")}
        </h5>
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
          <div>
            <Label>{t("companies:form.serverName")}</Label>
            <Input value={serverName} onChange={(e) => setServerName(e.target.value)} />
          </div>
          <div>
            <Label>{t("companies:form.databaseName")}</Label>
            <Input value={databaseName} onChange={(e) => setDatabaseName(e.target.value)} />
          </div>
          <div>
            <Label>{t("companies:form.serverUserId")}</Label>
            <Input value={serverUserId} onChange={(e) => setServerUserId(e.target.value)} />
          </div>
          <div>
            <Label>{t("companies:form.serverPassword")}</Label>
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
          {t("common:cancel")}
        </Button>
        <Button type="submit" disabled={isSubmitting}>{isSubmitting ? t("common:saving") : t("common:save")}</Button>
      </div>
    </form>
  );
}
