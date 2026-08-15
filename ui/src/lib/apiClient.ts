import type { ApiResponse } from "../types/api";
import type { AuthResponse } from "../types/auth";
import { ApiError } from "./apiError";
import i18next from "../i18n/config";

const BASE_URL = import.meta.env.VITE_API_BASE_URL as string;

const ACCESS_TOKEN_KEY = "accessToken";
const REFRESH_TOKEN_KEY = "refreshToken";
const ACCESS_TOKEN_EXPIRES_AT_KEY = "accessTokenExpiresAt";
const SELECTED_COMPANY_ID_KEY = "selectedCompanyId";

export function getAccessToken(): string | null {
  return localStorage.getItem(ACCESS_TOKEN_KEY);
}

export function getRefreshToken(): string | null {
  return localStorage.getItem(REFRESH_TOKEN_KEY);
}

export function setTokens(auth: AuthResponse): void {
  localStorage.setItem(ACCESS_TOKEN_KEY, auth.accessToken);
  localStorage.setItem(REFRESH_TOKEN_KEY, auth.refreshToken);
  localStorage.setItem(ACCESS_TOKEN_EXPIRES_AT_KEY, auth.accessTokenExpiresAt);
}

export function clearTokens(): void {
  localStorage.removeItem(ACCESS_TOKEN_KEY);
  localStorage.removeItem(REFRESH_TOKEN_KEY);
  localStorage.removeItem(ACCESS_TOKEN_EXPIRES_AT_KEY);
}

export function getSelectedCompanyId(): string | null {
  return localStorage.getItem(SELECTED_COMPANY_ID_KEY);
}

export function setSelectedCompanyId(companyId: string): void {
  localStorage.setItem(SELECTED_COMPANY_ID_KEY, companyId);
}

export function clearSelectedCompanyId(): void {
  localStorage.removeItem(SELECTED_COMPANY_ID_KEY);
}

let unauthorizedHandler: (() => void) | null = null;

export function setUnauthorizedHandler(handler: () => void): void {
  unauthorizedHandler = handler;
}

let refreshPromise: Promise<string> | null = null;

async function doRefresh(): Promise<string> {
  const refreshToken = getRefreshToken();
  if (!refreshToken) {
    throw new ApiError(i18next.t("common:errors.sessionExpired"), null, null, 401);
  }

  const res = await fetch(`${BASE_URL}/api/Auth/RefreshToken`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ refreshToken }),
  });
  const body: ApiResponse<AuthResponse> = await res.json();

  if (!body.success || !body.data) {
    clearTokens();
    throw new ApiError(body.message ?? i18next.t("common:errors.sessionRefreshFailed"), body.errorCode, body.errors, res.status);
  }

  setTokens(body.data);
  return body.data.accessToken;
}

function refreshAccessToken(): Promise<string> {
  if (!refreshPromise) {
    refreshPromise = doRefresh().finally(() => {
      refreshPromise = null;
    });
  }
  return refreshPromise;
}

function buildUrl(path: string, params?: Record<string, string | number | boolean | undefined>): string {
  const url = new URL(`${BASE_URL}${path}`);
  if (params) {
    for (const [key, value] of Object.entries(params)) {
      if (value !== undefined && value !== null && value !== "") {
        url.searchParams.set(key, String(value));
      }
    }
  }
  return url.toString();
}

interface RequestOptions {
  method: "GET" | "POST" | "PUT" | "DELETE";
  body?: unknown;
  params?: Record<string, string | number | boolean | undefined>;
}

async function request<T>(path: string, options: RequestOptions, isRetry = false): Promise<T> {
  const headers: Record<string, string> = { "Content-Type": "application/json" };

  const accessToken = getAccessToken();
  if (accessToken) {
    headers.Authorization = `Bearer ${accessToken}`;
  }

  const companyId = getSelectedCompanyId();
  if (companyId) {
    headers["X-Company-Id"] = companyId;
  }

  const res = await fetch(buildUrl(path, options.params), {
    method: options.method,
    headers,
    body: options.body !== undefined ? JSON.stringify(options.body) : undefined,
  });

  if (res.status === 401 && !isRetry && !path.startsWith("/api/Auth/")) {
    try {
      await refreshAccessToken();
      return await request<T>(path, options, true);
    } catch (err) {
      unauthorizedHandler?.();
      throw err;
    }
  }

  const body: ApiResponse<T> = await res.json();

  if (!body.success) {
    if (res.status === 401) {
      unauthorizedHandler?.();
    }
    throw new ApiError(body.message ?? i18next.t("common:errors.requestFailed"), body.errorCode, body.errors, res.status);
  }

  return body.data as T;
}

export function apiGet<T>(path: string, params?: Record<string, string | number | boolean | undefined>): Promise<T> {
  return request<T>(path, { method: "GET", params });
}

export function apiPost<T>(path: string, body?: unknown): Promise<T> {
  return request<T>(path, { method: "POST", body });
}

export function apiPut<T>(path: string, body?: unknown): Promise<T> {
  return request<T>(path, { method: "PUT", body });
}

export function apiDelete<T>(path: string): Promise<T> {
  return request<T>(path, { method: "DELETE" });
}
