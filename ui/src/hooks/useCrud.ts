import { useCallback, useEffect, useState } from "react";
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

  const [items, setItems] = useState<TListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize] = useState(20);
  const [searchTerm, setSearchTerm] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchList = useCallback(async () => {
    setIsLoading(true);
    setError(null);
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
      setError(err instanceof ApiError ? err.message : "Liste alınamadı.");
    } finally {
      setIsLoading(false);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [basePath, getListAction, pageNumber, pageSize, searchTerm]);

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
