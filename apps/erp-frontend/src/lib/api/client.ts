/**
 * Pharmacy ERP — Central API Client
 *
 * Strict single Axios instance configured according to Phase 0 contracts.
 * - Base URL: /api/v1 (resolved with origin fallback for Node/test environments)
 * - Automatic Bearer token injection from tokenStorage
 * - 401 refresh interceptor placeholder
 * - Standard ApiError envelope formatting
 */

import axios, {
  type AxiosError,
  type AxiosInstance,
  type InternalAxiosRequestConfig,
} from 'axios';
import { tokenStorage } from '../auth/tokenStorage';
import type { ApiError } from '../../types/api';

/**
 * Standard client error normalizing the backend ApiError envelope.
 */
export class ApiClientError extends Error {
  public readonly code: string;
  public readonly status?: number;
  public readonly details?: Array<{ field?: string; issue: string }>;
  public readonly trace_id?: string;

  constructor(payload: ApiError['error'], status?: number) {
    super(payload.message || 'An unexpected API error occurred');
    this.name = 'ApiClientError';
    this.code = payload.code || 'UNKNOWN_ERROR';
    this.status = status;
    this.details = payload.details;
    this.trace_id = payload.trace_id;
  }
}

/**
 * Custom request config interface extending Axios to track retry attempts.
 */
interface RetriableRequestConfig extends InternalAxiosRequestConfig {
  _retry?: boolean;
}

const resolveBaseUrl = (): string => {
  if (import.meta.env.VITE_API_BASE_URL) {
    return import.meta.env.VITE_API_BASE_URL;
  }
  // In Node.js testing environments, relative baseURLs cause Invalid URL errors.
  if (typeof window === 'undefined' || !window.location?.origin) {
    return 'http://localhost/api/v1';
  }
  return '/api/v1';
};

export const apiClient: AxiosInstance = axios.create({
  baseURL: resolveBaseUrl(),
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 15000,
});

// Request Interceptor: Inject JWT token from storage
apiClient.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = tokenStorage.getAccessToken();
    if (token && config.headers) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: unknown) => Promise.reject(error)
);

// Response Interceptor: Handle 401 refresh flow & ApiError formatting
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError<ApiError>) => {
    const originalRequest = error.config as RetriableRequestConfig | undefined;

    // 1. Placeholder for 401 Unauthorized token refresh logic
    if (
      error.response?.status === 401 &&
      originalRequest &&
      !originalRequest._retry
    ) {
      originalRequest._retry = true;

      const refreshToken = tokenStorage.getRefreshToken();
      if (refreshToken) {
        try {
          // TODO (Phase 1 Auth Flow):
          // Execute refresh mutation via POST /api/v1/auth/refresh:
          // const { data } = await axios.post<TokenPairResponse>(
          //   `${apiClient.defaults.baseURL}/auth/refresh`,
          //   { refresh_token: refreshToken }
          // );
          // tokenStorage.setTokens(data.access_token, data.refresh_token);
          // originalRequest.headers.Authorization = `Bearer ${data.access_token}`;
          // return apiClient(originalRequest);
        } catch (refreshError) {
          tokenStorage.clearTokens();
          // Dispatch session expired notification or redirect to /login
          return Promise.reject(refreshError);
        }
      } else {
        tokenStorage.clearTokens();
      }
    }

    // 2. Format error into standard ApiClientError if error envelope exists
    if (error.response?.data?.error) {
      return Promise.reject(
        new ApiClientError(error.response.data.error, error.response.status)
      );
    }

    return Promise.reject(error);
  }
);
