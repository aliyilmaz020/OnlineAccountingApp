import { useState } from "react";
import { Link, useNavigate } from "react-router";
import { useTranslation } from "react-i18next";
import { ChevronLeftIcon, EyeCloseIcon, EyeIcon } from "../../icons";
import Label from "../form/Label";
import Input from "../form/input/InputField";
import Checkbox from "../form/input/Checkbox";
import Button from "../ui/button/Button";
import Alert from "../ui/alert/Alert";
import { useAuth } from "../../context/AuthContext";
import { ApiError } from "../../lib/apiError";

export default function SignUpForm() {
  const { t } = useTranslation("auth");
  const { register } = useAuth();
  const navigate = useNavigate();

  const [showPassword, setShowPassword] = useState(false);
  const [isChecked, setIsChecked] = useState(false);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setIsSubmitting(true);
    try {
      await register(email, password);
      navigate("/select-company");
    } catch (err) {
      setError(err instanceof ApiError ? err.message : t("signUp.errorFallback"));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex flex-col flex-1 w-full overflow-y-auto lg:w-1/2 no-scrollbar">
      <div className="w-full max-w-md mx-auto mb-5 sm:pt-10">
        <Link
          to="/"
          className="inline-flex items-center text-sm text-gray-500 transition-colors hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300"
        >
          <ChevronLeftIcon className="size-5" />
          {t("backToDashboard")}
        </Link>
      </div>
      <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
        <div>
          <div className="mb-5 sm:mb-8">
            <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
              {t("signUp.heading")}
            </h1>
            <p className="text-sm text-gray-500 dark:text-gray-400">
              {t("signUp.subheading")}
            </p>
          </div>
          <div>
            {error && (
              <div className="mb-5">
                <Alert variant="error" title={t("signUp.errorTitle")} message={error} />
              </div>
            )}
            <form onSubmit={handleSubmit}>
              <div className="space-y-5">
                {/* <!-- Email --> */}
                <div>
                  <Label>
                    {t("signIn.email")}<span className="text-error-500">*</span>
                  </Label>
                  <Input
                    type="email"
                    id="email"
                    name="email"
                    placeholder={t("signUp.emailPlaceholder")}
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                  />
                </div>
                {/* <!-- Password --> */}
                <div>
                  <Label>
                    {t("signIn.password")}<span className="text-error-500">*</span>
                  </Label>
                  <div className="relative">
                    <Input
                      placeholder={t("signIn.passwordPlaceholder")}
                      type={showPassword ? "text" : "password"}
                      value={password}
                      onChange={(e) => setPassword(e.target.value)}
                    />
                    <span
                      onClick={() => setShowPassword(!showPassword)}
                      className="absolute z-30 -translate-y-1/2 cursor-pointer right-4 top-1/2"
                    >
                      {showPassword ? (
                        <EyeIcon className="fill-gray-500 dark:fill-gray-400 size-5" />
                      ) : (
                        <EyeCloseIcon className="fill-gray-500 dark:fill-gray-400 size-5" />
                      )}
                    </span>
                  </div>
                  <p className="mt-1.5 text-xs text-gray-500 dark:text-gray-400">
                    {t("signUp.passwordHint")}
                  </p>
                </div>
                {/* <!-- Checkbox --> */}
                <div className="flex items-center gap-3">
                  <Checkbox
                    className="w-5 h-5"
                    checked={isChecked}
                    onChange={setIsChecked}
                  />
                  <p className="inline-block font-normal text-gray-500 dark:text-gray-400">
                    {t("signUp.agreementPrefix")}{" "}
                    <span className="text-gray-800 dark:text-white/90">
                      {t("signUp.terms")}
                    </span>{" "}
                    {t("signUp.agreementMiddle")}{" "}
                    <span className="text-gray-800 dark:text-white">
                      {t("signUp.privacyPolicy")}
                    </span>
                  </p>
                </div>
                {/* <!-- Button --> */}
                <div>
                  <Button type="submit" className="w-full" size="sm" disabled={isSubmitting}>
                    {isSubmitting ? t("signUp.submitting") : t("signUp.submit")}
                  </Button>
                </div>
              </div>
            </form>

            <div className="mt-5">
              <p className="text-sm font-normal text-center text-gray-700 dark:text-gray-400 sm:text-start">
                {t("signUp.hasAccount")} {""}
                <Link
                  to="/signin"
                  className="text-brand-500 hover:text-brand-600 dark:text-brand-400"
                >
                  {t("signUp.signInLink")}
                </Link>
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
