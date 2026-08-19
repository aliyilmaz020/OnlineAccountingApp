import { useState } from "react";
import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import Label from "../../components/form/Label";
import Input from "../../components/form/input/InputField";
import Button from "../../components/ui/button/Button";
import Alert from "../../components/ui/alert/Alert";
import { apiPost } from "../../lib/apiClient";
import { ApiError } from "../../lib/apiError";
import type { ChangePasswordRequest } from "../../types/entities";

export default function AccountSettingsPage() {
  const { t } = useTranslation(["accountSettings", "common"]);
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmNewPassword, setConfirmNewPassword] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsSubmitting(true);
    setError(null);
    setSuccess(false);
    try {
      await apiPost("/api/Users/ChangePassword", {
        currentPassword,
        newPassword,
        confirmNewPassword,
      } satisfies ChangePasswordRequest);
      setSuccess(true);
      setCurrentPassword("");
      setNewPassword("");
      setConfirmNewPassword("");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : t("accountSettings:failed"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div>
      <PageMeta title={t("accountSettings:pageHeading")} description={t("accountSettings:pageDescription")} />
      <PageBreadcrumb pageTitle={t("accountSettings:pageHeading")} />
      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03] lg:p-6">
        <h3 className="mb-5 text-lg font-semibold text-gray-800 dark:text-white/90 lg:mb-7">
          {t("accountSettings:changePasswordTitle")}
        </h3>

        {success && (
          <div className="mb-4">
            <Alert variant="success" title={t("accountSettings:changePasswordTitle")} message={t("accountSettings:success")} />
          </div>
        )}
        {error && (
          <div className="mb-4">
            <Alert variant="error" title={t("common:errors.requestFailed")} message={error} />
          </div>
        )}

        <form onSubmit={handleSubmit} className="max-w-md space-y-4">
          <div>
            <Label>{t("accountSettings:currentPassword")}</Label>
            <Input type="password" value={currentPassword} onChange={(e) => setCurrentPassword(e.target.value)} />
          </div>
          <div>
            <Label>{t("accountSettings:newPassword")}</Label>
            <Input type="password" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} />
          </div>
          <div>
            <Label>{t("accountSettings:confirmNewPassword")}</Label>
            <Input
              type="password"
              value={confirmNewPassword}
              onChange={(e) => setConfirmNewPassword(e.target.value)}
            />
          </div>
          <div className="pt-2">
            <Button type="submit" disabled={isSubmitting}>
              {isSubmitting ? t("common:saving") : t("accountSettings:changePassword")}
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
}
