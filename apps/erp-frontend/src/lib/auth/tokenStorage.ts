/**
 * Pharmacy ERP — Token Storage
 *
 * The single authority for reading and writing auth tokens to browser storage.
 * Follows STRUCTURE.md: lib/auth/tokenStorage.ts is the only file that touches storage.
 * Includes in-memory fallback for SSR/Node/test environments where window.localStorage is unavailable.
 */

const ACCESS_TOKEN_KEY = 'erp_access_token';
const REFRESH_TOKEN_KEY = 'erp_refresh_token';

// In-memory fallback for environments without localStorage (e.g. Node test runners)
const inMemoryStore = new Map<string, string>();

const getStorage = (): {
  getItem: (key: string) => string | null;
  setItem: (key: string, value: string) => void;
  removeItem: (key: string) => void;
} => {
  if (
    typeof window !== 'undefined' &&
    typeof window.localStorage !== 'undefined'
  ) {
    return window.localStorage;
  }
  return {
    getItem: (key: string) => inMemoryStore.get(key) ?? null,
    setItem: (key: string, value: string) => inMemoryStore.set(key, value),
    removeItem: (key: string) => inMemoryStore.delete(key),
  };
};

export const tokenStorage = {
  getAccessToken(): string | null {
    try {
      return getStorage().getItem(ACCESS_TOKEN_KEY);
    } catch {
      return inMemoryStore.get(ACCESS_TOKEN_KEY) ?? null;
    }
  },

  getRefreshToken(): string | null {
    try {
      return getStorage().getItem(REFRESH_TOKEN_KEY);
    } catch {
      return inMemoryStore.get(REFRESH_TOKEN_KEY) ?? null;
    }
  },

  setTokens(accessToken: string, refreshToken?: string | null): void {
    try {
      const storage = getStorage();
      storage.setItem(ACCESS_TOKEN_KEY, accessToken);
      if (refreshToken) {
        storage.setItem(REFRESH_TOKEN_KEY, refreshToken);
      }
    } catch (error) {
      console.warn('Unable to persist auth tokens to storage', error);
      inMemoryStore.set(ACCESS_TOKEN_KEY, accessToken);
      if (refreshToken) {
        inMemoryStore.set(REFRESH_TOKEN_KEY, refreshToken);
      }
    }
  },

  clearTokens(): void {
    try {
      const storage = getStorage();
      storage.removeItem(ACCESS_TOKEN_KEY);
      storage.removeItem(REFRESH_TOKEN_KEY);
    } catch (error) {
      console.warn('Unable to clear auth tokens from storage', error);
    } finally {
      inMemoryStore.clear();
    }
  },
};
