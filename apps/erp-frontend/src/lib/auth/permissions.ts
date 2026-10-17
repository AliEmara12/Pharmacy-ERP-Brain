/**
 * Pharmacy ERP — Roles & Permissions Matrix
 *
 * Strictly aligned with contracts/schemas/common.json:
 * Closed enum Role = 'Admin' | 'Pharmacist' | 'Cashier'.
 * No 'Staff' or ad-hoc role strings permitted.
 */

import type { Role, PermissionScope } from '../../types/api';

export const VALID_ROLES: readonly Role[] = [
  'Admin',
  'Pharmacist',
  'Cashier',
] as const;

export const ROLE_PERMISSIONS: Record<Role, readonly PermissionScope[]> = {
  Admin: [
    'user:admin',
    'inventory:read',
    'inventory:write',
    'sales:read',
    'sales:write',
    'return:read',
    'return:write',
    'purchase-order:read',
    'purchase-order:draft',
    'purchase-order:approve',
    'purchase-order:receive',
    'supplier-payment:write',
    'sync:retry',
  ],
  Pharmacist: [
    'inventory:read',
    'inventory:write',
    'sales:read',
    'return:read',
    'purchase-order:read',
    'purchase-order:draft',
    'purchase-order:receive',
    'supplier-payment:write',
  ],
  Cashier: [
    'sales:read',
    'sales:write',
    'return:read',
    'return:write',
    'inventory:read',
  ],
} as const;

export function isValidRole(role: string): role is Role {
  return (VALID_ROLES as readonly string[]).includes(role);
}

export function hasRole(
  userRoles: readonly Role[] | undefined,
  requiredRole: Role
): boolean {
  if (!userRoles || userRoles.length === 0) return false;
  return userRoles.includes(requiredRole);
}

export function hasAnyRole(
  userRoles: readonly Role[] | undefined,
  requiredRoles: readonly Role[]
): boolean {
  if (!userRoles || userRoles.length === 0) return false;
  return requiredRoles.some((role) => userRoles.includes(role));
}

export function hasPermission(
  userScopes: readonly PermissionScope[] | undefined,
  requiredScope: PermissionScope
): boolean {
  if (!userScopes || userScopes.length === 0) return false;
  return userScopes.includes(requiredScope);
}

export function hasAllPermissions(
  userScopes: readonly PermissionScope[] | undefined,
  requiredScopes: readonly PermissionScope[]
): boolean {
  if (!userScopes || userScopes.length === 0) return false;
  return requiredScopes.every((scope) => userScopes.includes(scope));
}
