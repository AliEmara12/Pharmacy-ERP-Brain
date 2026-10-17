/**
 * Pharmacy ERP — MSW Request Handlers
 *
 * Mock Service Worker endpoints matching Phase 0 contracts.
 * Handlers return canonical JSON mock payloads from contracts/fixtures.
 */

import { http, HttpResponse } from 'msw';
import authTokenPair from '@contracts/fixtures/auth-token-pair.json';
import currentUserContext from '@contracts/fixtures/current-user-context.json';

export const handlers = [
  // POST /api/v1/auth/login
  http.post('*/api/v1/auth/login', async () => {
    return HttpResponse.json(authTokenPair, { status: 200 });
  }),

  // GET /api/v1/auth/me
  http.get('*/api/v1/auth/me', () => {
    return HttpResponse.json(currentUserContext, { status: 200 });
  }),

  // POST /api/v1/auth/logout
  http.post('*/api/v1/auth/logout', () => {
    return new HttpResponse(null, { status: 204 });
  }),
];
