import { useEffect, useState } from "react";
import { apiGet } from "../lib/apiClient";
import type { PagedResult } from "../types/api";

export interface Option {
  value: string;
  label: string;
}

export function useLookupOptions<T extends { id: string }>(
  basePath: string,
  listAction: string,
  labelSelector: (item: T) => string,
) {
  const [options, setOptions] = useState<Option[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    setIsLoading(true);
    apiGet<PagedResult<T>>(`${basePath}/${listAction}`, { pageNumber: 1, pageSize: 100 })
      .then((result) => {
        if (!cancelled) {
          setOptions(result.items.map((item) => ({ value: item.id, label: labelSelector(item) })));
        }
      })
      .finally(() => {
        if (!cancelled) setIsLoading(false);
      });
    return () => {
      cancelled = true;
    };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [basePath, listAction]);

  return { options, isLoading };
}
