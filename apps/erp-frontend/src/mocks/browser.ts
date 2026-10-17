/**
 * Pharmacy ERP — MSW Browser Worker
 *
 * Configures Mock Service Worker for client-side API interception in dev/mock mode.
 */

import { setupWorker } from 'msw/browser';
import { handlers } from './handlers';

export const worker = setupWorker(...handlers);
