import { useTranslation } from "react-i18next";

const LANGUAGE_KEY = "language";

export const LanguageToggleButton: React.FC = () => {
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
      className="relative flex items-center justify-center text-gray-500 transition-colors bg-white border border-gray-200 rounded-full hover:text-dark-900 h-11 w-11 hover:bg-gray-100 hover:text-gray-700 dark:border-gray-800 dark:bg-gray-900 dark:text-gray-400 dark:hover:bg-gray-800 dark:hover:text-white"
    >
      <span className="text-xs font-semibold uppercase">
        {i18n.language === "tr" ? "EN" : "TR"}
      </span>
    </button>
  );
};
