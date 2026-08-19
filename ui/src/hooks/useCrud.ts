import { useCallback, useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import { apiDelete, apiGet, apiPost, apiPut } from "../lib/apiClient";
import { ApiError } from "../lib/apiError";
import type { PagedResult } from "../types/api";

export interface CrudConfig {
  basePath: string;
  getListAction: string;
  createAction: string;
  updateAction: string;
  deleteAction: string;
}

export function useCrud<
  TListItem extends { id: string },
  TCreateReq,
  TUpdateReq,
>(config: CrudConfig) {
  const { basePath, getListAction, createAction, updateAction, deleteAction } = config;
  const { t } = useTranslation("common");

  const [items, setItems] = useState<TListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(20);
  const [searchTerm, setSearchTerm] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  // apiErrorMessage holds server-provided text as-is; hasGenericError means the client-side
  // fallback should be shown - resolved via t() at render time so it updates instantly on
  // language switch, instead of freezing whatever language was active when the error occurred.
  const [apiErrorMessage, setApiErrorMessage] = useState<string | null>(null);
  const [hasGenericError, setHasGenericError] = useState(false);

  const fetchList = useCallback(async () => {
    setIsLoading(true);
    setApiErrorMessage(null);
    setHasGenericError(false);
    try {
      const result = await apiGet<PagedResult<TListItem>>(`${basePath}/${getListAction}`, {
        pageNumber,
        pageSize,
        searchTerm: searchTerm || undefined,
      });
      setItems(result.items);
      setTotalCount(result.totalCount);
      setTotalPages(result.totalPages);
    } catch (err) {
      if (err instanceof ApiError) {
        setApiErrorMessage(err.message);
      } else {
        setHasGenericError(true);
      }
    } finally {
      setIsLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [basePath, getListAction, pageNumber, pageSize, searchTerm]);

  const error = hasGenericError ? t("errors.listFetchFailed") : apiErrorMessage;

  useEffect(() => {
    fetchList();
  }, [fetchList]);

  const create = useCallback(
    async (payload: TCreateReq) => {
      await apiPost(`${basePath}/${createAction}`, payload);
      await fetchList();
    },
    [basePath, createAction, fetchList],
  );

  const update = useCallback(
    async (id: string, payload: TUpdateReq) => {
      await apiPut(`${basePath}/${updateAction}/${id}`, payload);
      await fetchList();
    },
    [basePath, updateAction, fetchList],
  );

  const remove = useCallback(
    async (id: string) => {
      await apiDelete(`${basePath}/${deleteAction}/${id}`);
      await fetchList();
    },
    [basePath, deleteAction, fetchList],
  );

  return {
    items,
    totalCount,
    totalPages,
    pageNumber,
    setPageNumber,
    pageSize,
    searchTerm,
    setSearchTerm,
    isLoading,
    error,
    create,
    update,
    remove,
    refetch: fetchList,
  };
}
