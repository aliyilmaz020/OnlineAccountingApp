import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { useModal } from "../../hooks/useModal";
import { useAuth } from "../../context/AuthContext";
import { apiGet, apiPut } from "../../lib/apiClient";
import { ApiError } from "../../lib/apiError";
import { Modal } from "../ui/modal";
import Button from "../ui/button/Button";
import Alert from "../ui/alert/Alert";
import Input from "../form/input/InputField";
import Label from "../form/Label";
import type { MyProfile, UpdateMyProfileRequest } from "../../types/entities";

export default function ProfileCard() {
  const { t } = useTranslation(["profile", "common"]);
  const { refreshUser } = useAuth();
  const { isOpen, openModal, closeModal } = useModal();

  const [profile, setProfile] = useState<MyProfile | null>(null);
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [userName, setUserName] = useState("");
  const [email, setEmail] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  useEffect(() => {
    apiGet<MyProfile>("/api/Users/GetMyProfile").then((result) => {
      setProfile(result);
      setFirstName(result.firstName ?? "");
      setLastName(result.lastName ?? "");
      setUserName(result.userName ?? "");
      setEmail(result.email ?? "");
      setPhoneNumber(result.phoneNumber ?? "");
    });
  }, []);

  const handleOpenModal = () => {
    setFormError(null);
    setFirstName(profile?.firstName ?? "");
    setLastName(profile?.lastName ?? "");
    setUserName(profile?.userName ?? "");
    setEmail(profile?.email ?? "");
    setPhoneNumber(profile?.phoneNumber ?? "");
    openModal();
  };

  const handleSave = async () => {
    setIsSubmitting(true);
    setFormError(null);
    try {
      const updated = await apiPut<MyProfile>("/api/Users/UpdateMyProfile", {
        userName,
        email,
        phoneNumber: phoneNumber || undefined,
        firstName: firstName || undefined,
        lastName: lastName || undefined,
      } satisfies UpdateMyProfileRequest);
      setProfile(updated);
      await refreshUser();
      closeModal();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : t("profile:updateFailed"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="p-5 border border-gray-200 rounded-2xl dark:border-gray-800 lg:p-6">
      <div className="flex flex-col gap-6 lg:flex-row lg:items-start lg:justify-between">
        <div>
          <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90 lg:mb-6">
            {t("profile:personalInfo")}
          </h4>

          <div className="grid grid-cols-1 gap-4 lg:grid-cols-2 lg:gap-7 2xl:gap-x-32">
            <div>
              <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">
                {t("profile:firstName")}
              </p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{profile?.firstName}</p>
            </div>
            <div>
              <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">
                {t("profile:lastName")}
              </p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{profile?.lastName}</p>
            </div>
            <div>
              <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">
                {t("profile:userName")}
              </p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{profile?.userName}</p>
            </div>
            <div>
              <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">
                {t("profile:emailAddress")}
              </p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{profile?.email}</p>
            </div>
            <div>
              <p className="mb-2 text-xs leading-normal text-gray-500 dark:text-gray-400">{t("profile:phone")}</p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{profile?.phoneNumber}</p>
            </div>
          </div>
        </div>

        <button
          onClick={handleOpenModal}
          className="flex w-full items-center justify-center gap-2 rounded-full border border-gray-300 bg-white px-4 py-3 text-sm font-medium text-gray-700 shadow-theme-xs hover:bg-gray-50 hover:text-gray-800 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-400 dark:hover:bg-white/[0.03] dark:hover:text-gray-200 lg:inline-flex lg:w-auto"
        >
          {t("profile:edit")}
        </button>
      </div>

      <Modal isOpen={isOpen} onClose={closeModal} className="max-w-[600px] m-4">
        <div className="no-scrollbar relative w-full max-w-[600px] overflow-y-auto rounded-3xl bg-white p-4 dark:bg-gray-900 lg:p-11">
          <div className="px-2 pr-14">
            <h4 className="mb-2 text-2xl font-semibold text-gray-800 dark:text-white/90">
              {t("profile:editPersonalInfo")}
            </h4>
            <p className="mb-6 text-sm text-gray-500 dark:text-gray-400 lg:mb-7">{t("profile:updateDetails")}</p>
          </div>
          {formError && (
            <div className="mb-4 px-2">
              <Alert variant="error" title={t("common:errors.requestFailed")} message={formError} />
            </div>
          )}
          <div className="flex flex-col">
            <div className="grid grid-cols-1 gap-x-6 gap-y-5 px-2 pb-3 sm:grid-cols-2">
              <div>
                <Label>{t("profile:firstName")}</Label>
                <Input value={firstName} onChange={(e) => setFirstName(e.target.value)} />
              </div>
              <div>
                <Label>{t("profile:lastName")}</Label>
                <Input value={lastName} onChange={(e) => setLastName(e.target.value)} />
              </div>
              <div className="sm:col-span-2">
                <Label>{t("profile:userName")}</Label>
                <Input value={userName} onChange={(e) => setUserName(e.target.value)} />
              </div>
              <div>
                <Label>{t("profile:emailAddress")}</Label>
                <Input type="email" value={email} onChange={(e) => setEmail(e.target.value)} />
              </div>
              <div>
                <Label>{t("profile:phone")}</Label>
                <Input value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} />
              </div>
            </div>
            <div className="flex items-center gap-3 px-2 mt-6 lg:justify-end">
              <Button size="sm" variant="outline" onClick={closeModal}>
                {t("common:cancel")}
              </Button>
              <Button size="sm" onClick={handleSave} disabled={isSubmitting}>
                {isSubmitting ? t("common:saving") : t("common:save")}
              </Button>
            </div>
          </div>
        </div>
      </Modal>
    </div>
  );
}
