import { useTranslation } from "react-i18next";

const LANGUAGE_KEY = "language";

export default function LanguageTogglerTwo() {
  const { i18n } = useTranslation();

  const toggleLanguage = () => {
    const next = i18n.language === "tr" ? "en" : "tr";
    i18n.changeLanguage(next);
    localStorage.setItem(LANGUAGE_KEY, next);
  };

  return (
    <button
      onClick={toggleLanguage}
      aria-label="Toggle language"
      className="inline-flex items-center justify-center text-white transition-colors rounded-full size-14 bg-brand-500 hover:bg-brand-600"
    >
      <span className="text-xs font-semibold uppercase">
        {i18n.language === "tr" ? "EN" : "TR"}
      </span>
    </button>
  );
}
