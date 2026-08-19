import { useEffect, useState } from "react";
import { Link } from "react-router";
import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import { GroupIcon, PageIcon, ListIcon, TableIcon, UserCircleIcon } from "../../icons";
import { apiGet } from "../../lib/apiClient";
import { useCompany } from "../../context/CompanyContext";
import type { PagedResult } from "../../types/api";
import type { Company, MainRole, Role, UniformChartOfAccount, UserListItem } from "../../types/entities";

interface StatCardConfig {
  key: string;
  labelKey: string;
  path: string;
  icon: React.ReactNode;
}

const cards: StatCardConfig[] = [
  { key: "companies", labelKey: "dashboard:stats.companies", path: "/companies", icon: <PageIcon className="size-6" /> },
  { key: "users", labelKey: "dashboard:stats.users", path: "/users", icon: <UserCircleIcon className="size-6" /> },
  { key: "roles", labelKey: "dashboard:stats.roles", path: "/roles", icon: <ListIcon className="size-6" /> },
  { key: "mainRoles", labelKey: "dashboard:stats.mainRoles", path: "/main-roles", icon: <GroupIcon className="size-6" /> },
  {
    key: "uniformChartOfAccounts",
    labelKey: "dashboard:stats.uniformChartOfAccounts",
    path: "/uniform-chart-of-accounts",
    icon: <TableIcon className="size-6" />,
  },
];

export default function Home() {
  const { t: tPageTitles } = useTranslation("pageTitles");
  const { t } = useTranslation("dashboard");
  const { selectedCompanyId } = useCompany();
  const [counts, setCounts] = useState<Record<string, number | null>>({});

  useEffect(() => {
    let cancelled = false;

    const load = async () => {
      const [companies, users, roles, mainRoles] = await Promise.all([
        apiGet<PagedResult<Company>>("/api/Companies/GetCompanies", { pageNumber: 1, pageSize: 1 }),
        apiGet<PagedResult<UserListItem>>("/api/Users/GetUsers", { pageNumber: 1, pageSize: 1 }),
        apiGet<PagedResult<Role>>("/api/Roles/GetRoles", { pageNumber: 1, pageSize: 1 }),
        apiGet<PagedResult<MainRole>>("/api/MainRoles/GetMainRoles", { pageNumber: 1, pageSize: 1 }),
      ]);

      if (cancelled) return;
      setCounts((prev) => ({
        ...prev,
        companies: companies.totalCount,
        users: users.totalCount,
        roles: roles.totalCount,
        mainRoles: mainRoles.totalCount,
      }));

      if (selectedCompanyId) {
        const uniformChartOfAccounts = await apiGet<PagedResult<UniformChartOfAccount>>(
          "/api/UniformChartOfAccounts/GetUniformChartOfAccounts",
          { pageNumber: 1, pageSize: 1 },
        );
        if (!cancelled) {
          setCounts((prev) => ({ ...prev, uniformChartOfAccounts: uniformChartOfAccounts.totalCount }));
        }
      } else {
        setCounts((prev) => ({ ...prev, uniformChartOfAccounts: null }));
      }
    };

    load();
    return () => {
      cancelled = true;
    };
  }, [selectedCompanyId]);

  return (
    <>
      <PageMeta title={tPageTitles("dashboard")} description={t("pageDescription")} />
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 md:gap-6 xl:grid-cols-3">
        {cards.map((card) => {
          const count = counts[card.key];
          const isDisabled = card.key === "uniformChartOfAccounts" && !selectedCompanyId;
          return (
            <Link
              key={card.key}
              to={card.path}
              className="rounded-2xl border border-gray-200 bg-white p-5 transition hover:border-brand-300 dark:border-gray-800 dark:bg-white/[0.03] dark:hover:border-brand-800 md:p-6"
            >
              <div className="flex items-center justify-center w-12 h-12 rounded-xl bg-gray-100 text-gray-700 dark:bg-gray-800 dark:text-gray-300">
                {card.icon}
              </div>
              <div className="mt-5">
                <span className="text-sm text-gray-500 dark:text-gray-400">{t(card.labelKey)}</span>
                <h4 className="mt-2 text-title-sm font-bold text-gray-800 dark:text-white/90">
                  {isDisabled ? t("stats.selectCompanyHint") : (count ?? "…")}
                </h4>
              </div>
            </Link>
          );
        })}
      </div>
    </>
  );
}
