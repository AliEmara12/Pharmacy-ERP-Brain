import { describe, it, expect, beforeAll, afterEach, afterAll } from 'vitest';
import { setupServer } from 'msw/node';
import { handlers } from '../mocks/handlers';
import { authService } from '../features/auth/api/authService';
import { tokenStorage } from '../lib/auth/tokenStorage';

const server = setupServer(...handlers);

beforeAll(() => server.listen());
afterEach(() => {
  server.resetHandlers();
  tokenStorage.clearTokens();
});
afterAll(() => server.close());

describe('authService & MSW integration', () => {
  it('login() authenticates and returns canonical TokenPairResponse fixture', async () => {
    const result = await authService.login({
      organization_code: 'PHARMAPLUS',
      username: 'ahmed.pharmacist',
      password: 'password123',
    });

    expect(result.access_token).toBeDefined();
    expect(result.token_type).toBe('Bearer');
    expect(result.user.role).toBe('Pharmacist');
    expect(tokenStorage.getAccessToken()).toBe(result.access_token);
  });

  it('getCurrentUser() retrieves canonical CurrentUserContext fixture', async () => {
    const ctx = await authService.getCurrentUser();

    expect(ctx.user.username).toBe('fa.pharmacist');
    expect(ctx.roles).toContain('Pharmacist');
    expect(ctx.organization_code).toBe('PHARMAPLUS');
    expect(ctx.runtime).toBe('BranchLocal');
  });

  it('logout() clears tokens from storage', async () => {
    tokenStorage.setTokens('sample_access', 'sample_refresh');
    await authService.logout();
    expect(tokenStorage.getAccessToken()).toBeNull();
    expect(tokenStorage.getRefreshToken()).toBeNull();
  });
});
