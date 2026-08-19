import { useState } from "react";
import { useTranslation } from "react-i18next";
import PageMeta from "../../components/common/PageMeta";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import Alert from "../../components/ui/alert/Alert";
import Button from "../../components/ui/button/Button";
import { apiGet, apiPost } from "../../lib/apiClient";
import { ApiError } from "../../lib/apiError";
import type { Role, SeedSampleDataResult } from "../../types/entities";

interface ToolResult {
  variant: "success" | "error";
  message: string;
}

function ToolCard({
  title,
  description,
  isRunning,
  result,
  onRun,
  runLabel,
  runningLabel,
  children,
}: {
  title: string;
  description: string;
  isRunning: boolean;
  result: ToolResult | null;
  onRun: () => void;
  runLabel: string;
  runningLabel: string;
  children?: React.ReactNode;
}) {
  return (
    <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
      <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90">{title}</h3>
      <p className="mt-1 text-sm text-gray-500 dark:text-gray-400">{description}</p>
      <div className="mt-4">
        <Button size="sm" disabled={isRunning} onClick={onRun}>
          {isRunning ? runningLabel : runLabel}
        </Button>
      </div>
      {result && (
        <div className="mt-4">
          <Alert
            variant={result.variant}
            title={result.variant === "success" ? title : "Error"}
            message={result.message}
          />
        </div>
      )}
      {children}
    </div>
  );
}

export default function SystemToolsPage() {
  const { t } = useTranslation(["systemTools", "common"]);

  const [migrateRunning, setMigrateRunning] = useState(false);
  const [migrateResult, setMigrateResult] = useState<ToolResult | null>(null);

  const [createRolesRunning, setCreateRolesRunning] = useState(false);
  const [createRolesResult, setCreateRolesResult] = useState<ToolResult | null>(null);

  const [seedRunning, setSeedRunning] = useState(false);
  const [seedResult, setSeedResult] = useState<ToolResult | null>(null);
  const [seedData, setSeedData] = useState<SeedSampleDataResult | null>(null);

  const runMigrateDb = async () => {
    setMigrateRunning(true);
    setMigrateResult(null);
    try {
      await apiGet<boolean>("/api/Companies/MigrateCompanyDb");
      setMigrateResult({ variant: "success", message: t("systemTools:migrateDb.success") });
    } catch (err) {
      setMigrateResult({ variant: "error", message: err instanceof ApiError ? err.message : t("common:errors.requestFailed") });
    } finally {
      setMigrateRunning(false);
    }
  };

  const runCreateAllRoles = async () => {
    setCreateRolesRunning(true);
    setCreateRolesResult(null);
    try {
      const created = await apiPost<Role[]>("/api/Roles/CreateAllRoles");
      const message =
        created.length === 0
          ? t("systemTools:createAllRoles.successNone")
          : t("systemTools:createAllRoles.successSome", {
              count: created.length,
              names: created.map((r) => r.name).join(", "),
            });
      setCreateRolesResult({ variant: "success", message });
    } catch (err) {
      setCreateRolesResult({ variant: "error", message: err instanceof ApiError ? err.message : t("common:errors.requestFailed") });
    } finally {
      setCreateRolesRunning(false);
    }
  };

  const runSeedSampleData = async () => {
    setSeedRunning(true);
    setSeedResult(null);
    setSeedData(null);
    try {
      const result = await apiPost<SeedSampleDataResult>("/api/Seed/SeedSampleData");
      setSeedData(result);
      setSeedResult({ variant: "success", message: t("systemTools:seedSampleData.success") });
    } catch (err) {
      setSeedResult({ variant: "error", message: err instanceof ApiError ? err.message : t("common:errors.requestFailed") });
    } finally {
      setSeedRunning(false);
    }
  };

  return (
    <div>
      <PageMeta title={t("systemTools:title")} description={t("systemTools:description")} />
      <PageBreadcrumb pageTitle={t("systemTools:title")} />

      <div className="grid grid-cols-1 gap-5 lg:grid-cols-3">
        <ToolCard
          title={t("systemTools:migrateDb.title")}
          description={t("systemTools:migrateDb.description")}
          isRunning={migrateRunning}
          result={migrateResult}
          onRun={runMigrateDb}
          runLabel={t("systemTools:run")}
          runningLabel={t("systemTools:running")}
        />

        <ToolCard
          title={t("systemTools:createAllRoles.title")}
          description={t("systemTools:createAllRoles.description")}
          isRunning={createRolesRunning}
          result={createRolesResult}
          onRun={runCreateAllRoles}
          runLabel={t("systemTools:run")}
          runningLabel={t("systemTools:running")}
        />

        <ToolCard
          title={t("systemTools:seedSampleData.title")}
          description={t("systemTools:seedSampleData.description")}
          isRunning={seedRunning}
          result={seedResult}
          onRun={runSeedSampleData}
          runLabel={t("systemTools:run")}
          runningLabel={t("systemTools:running")}
        >
          {seedData && (
            <ul className="mt-4 space-y-1 text-sm text-gray-600 dark:text-gray-300">
              <li>
                {t("systemTools:seedSampleData.results.permissionRolesCreated")}: {seedData.permissionRolesCreated}
              </li>
              <li>
                {t("systemTools:seedSampleData.results.companiesCreated")}: {seedData.companiesCreated}
              </li>
              <li>
                {t("systemTools:seedSampleData.results.usersCreated")}: {seedData.usersCreated}
              </li>
              <li>
                {t("systemTools:seedSampleData.results.userCompanyLinksCreated")}: {seedData.userCompanyLinksCreated}
              </li>
              <li>
                {t("systemTools:seedSampleData.results.mainRolesCreated")}: {seedData.mainRolesCreated}
              </li>
              <li>
                {t("systemTools:seedSampleData.results.mainRoleRoleLinksCreated")}: {seedData.mainRoleRoleLinksCreated}
              </li>
              <li>
                {t("systemTools:seedSampleData.results.mainRoleUserLinksCreated")}: {seedData.mainRoleUserLinksCreated}
              </li>
            </ul>
          )}
        </ToolCard>
      </div>
    </div>
  );
}
