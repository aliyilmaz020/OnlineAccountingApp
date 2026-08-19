import { useCallback, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import PageBreadcrumb from "../../components/common/PageBreadCrumb";
import PageMeta from "../../components/common/PageMeta";
import Input from "../../components/form/input/InputField";
import Alert from "../../components/ui/alert/Alert";
import Button from "../../components/ui/button/Button";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../../components/ui/table";
import { apiGet } from "../../lib/apiClient";
import type { PagedResult } from "../../types/api";
import type { UserListItem } from "../../types/entities";
import ManageUserRolesModal from "./ManageUserRolesModal";

export default function UsersListPage() {
  const { t } = useTranslation(["users", "common", "crud"]);
  const [items, setItems] = useState<UserListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [hasError, setHasError] = useState(false);
  const [managingUser, setManagingUser] = useState<UserListItem | null>(null);

  const fetchList = useCallback(async () => {
    setIsLoading(true);
    setHasError(false);
    try {
      const result = await apiGet<PagedResult<UserListItem>>("/api/Users/GetUsers", {
        pageNumber,
        pageSize: 20,
        searchTerm: searchTerm || undefined,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch {
      setHasError(true);
    } finally {
      setIsLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [pageNumber, searchTerm]);

  useEffect(() => {
    fetchList();
  }, [fetchList]);

  return (
    <div>
      <PageMeta title={t("users:title")} description={t("users:description")} />
      <PageBreadcrumb pageTitle={t("users:title")} />

      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
        <div className="mb-4 flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
          <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90">{t("users:title")}</h3>
          <Input
            placeholder={t("crud:searchPlaceholder")}
            value={searchTerm}
            onChange={(e) => {
              setPageNumber(1);
              setSearchTerm(e.target.value);
            }}
          />
        </div>

        {hasError && (
          <div className="mb-4">
            <Alert variant="error" title={t("crud:errorTitle")} message={t("common:errors.listFetchFailed")} />
          </div>
        )}

        <div className="overflow-x-auto">
          <Table>
            <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
              <TableRow>
                <TableCell isHeader className="px-4 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-400">
                  {t("users:columns.userName")}
                </TableCell>
                <TableCell isHeader className="px-4 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-400">
                  {t("users:columns.email")}
                </TableCell>
                <TableCell isHeader className="px-4 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-400">
                  {t("users:columns.status")}
                </TableCell>
                <TableCell isHeader className="px-4 py-3 text-start text-xs font-medium text-gray-500 dark:text-gray-400">
                  {t("crud:actions")}
                </TableCell>
              </TableRow>
            </TableHeader>
            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {isLoading ? (
                <TableRow>
                  <TableCell className="px-4 py-4 text-gray-500" colSpan={4}>
                    {t("crud:loading")}
                  </TableCell>
                </TableRow>
              ) : items.length === 0 ? (
                <TableRow>
                  <TableCell className="px-4 py-4 text-gray-500" colSpan={4}>
                    {t("crud:noRecords")}
                  </TableCell>
                </TableRow>
              ) : (
                items.map((user) => (
                  <TableRow key={user.id}>
                    <TableCell className="px-4 py-3 text-sm text-gray-700 dark:text-gray-300">{user.userName}</TableCell>
                    <TableCell className="px-4 py-3 text-sm text-gray-700 dark:text-gray-300">{user.email}</TableCell>
                    <TableCell className="px-4 py-3 text-sm text-gray-700 dark:text-gray-300">
                      {user.status ? t("common:active") : t("common:inactive")}
                    </TableCell>
                    <TableCell className="px-4 py-3 text-sm">
                      <button className="text-brand-500 hover:text-brand-600" onClick={() => setManagingUser(user)}>
                        {t("users:manageRoles")}
                      </button>
                    </TableCell>
                  </TableRow>
                ))
              )}
            </TableBody>
          </Table>
        </div>

        <div className="mt-4 flex items-center justify-between text-sm text-gray-500 dark:text-gray-400">
          <span>{t("crud:totalCount", { count: totalCount })}</span>
          <div className="flex items-center gap-2">
            <Button size="sm" variant="outline" disabled={pageNumber <= 1} onClick={() => setPageNumber(pageNumber - 1)}>
              {t("crud:previous")}
            </Button>
            <span>{t("crud:pageIndicator", { page: pageNumber, totalPages: Math.max(totalPages, 1) })}</span>
            <Button
              size="sm"
              variant="outline"
              disabled={pageNumber >= totalPages}
              onClick={() => setPageNumber(pageNumber + 1)}
            >
              {t("crud:next")}
            </Button>
          </div>
        </div>
      </div>

      {managingUser && <ManageUserRolesModal user={managingUser} onClose={() => setManagingUser(null)} />}
    </div>
  );
}
