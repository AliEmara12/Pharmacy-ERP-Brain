import { describe, it, expect, beforeEach, beforeAll, afterAll } from 'vitest';
import { setupServer } from 'msw/node';
import { handlers } from '../mocks/handlers';
import { useAuthStore } from '../stores/authStore';
import { tokenStorage } from '../lib/auth/tokenStorage';

import currentUserContextFixture from '@contracts/fixtures/current-user-context.json';
import type { CurrentUserContext } from '../types/api';

const server = setupServer(...handlers);

beforeAll(() => server.listen());
afterAll(() => server.close());

beforeEach(() => {
  server.resetHandlers();
  tokenStorage.clearTokens();
  useAuthStore.setState({
    currentUser: null,
    isAuthenticated: false,
    isInitializing: true,
  });
});

describe('authStore', () => {
  it('initialize() sets isAuthenticated: false when no token is present', async () => {
    await useAuthStore.getState().initialize();

    const state = useAuthStore.getState();
    expect(state.isInitializing).toBe(false);
    expect(state.isAuthenticated).toBe(false);
    expect(state.currentUser).toBeNull();
  });

  it('login() authenticates, fetches user context, and updates state', async () => {
    await useAuthStore.getState().login({
      organization_code: 'PHARMAPLUS',
      username: 'ahmed.pharmacist',
      password: 'password123',
    });

    const state = useAuthStore.getState();
    expect(state.isInitializing).toBe(false);
    expect(state.isAuthenticated).toBe(true);
    expect(state.currentUser).not.toBeNull();
    expect(state.currentUser?.user.username).toBe('fa.pharmacist');
    expect(state.currentUser?.roles).toContain('Pharmacist');
    expect(tokenStorage.getAccessToken()).toBeDefined();
  });

  it('initialize() restores session when valid token exists in storage', async () => {
    tokenStorage.setTokens('existing_valid_token');

    await useAuthStore.getState().initialize();

    const state = useAuthStore.getState();
    expect(state.isInitializing).toBe(false);
    expect(state.isAuthenticated).toBe(true);
    expect(state.currentUser?.user.username).toBe('fa.pharmacist');
  });

  it('logout() resets store state and clears tokens', async () => {
    tokenStorage.setTokens('test_token');
    useAuthStore.setState({
      currentUser: currentUserContextFixture as CurrentUserContext,
      isAuthenticated: true,
      isInitializing: false,
    });

    await useAuthStore.getState().logout();

    const state = useAuthStore.getState();
    expect(state.isAuthenticated).toBe(false);
    expect(state.currentUser).toBeNull();
    expect(tokenStorage.getAccessToken()).toBeNull();
  });
});
