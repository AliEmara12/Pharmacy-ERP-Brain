import { describe, it, expect } from 'vitest';
import {
  VALID_ROLES,
  ROLE_PERMISSIONS,
  isValidRole,
  hasRole,
  hasAnyRole,
  hasPermission,
  hasAllPermissions,
} from '../lib/auth/permissions';
import type { Role } from '../types/api';

describe('Roles and Permissions Matrix', () => {
  it('strictly contains only the three canonical contract roles', () => {
    expect(VALID_ROLES).toEqual(['Admin', 'Pharmacist', 'Cashier']);
    expect(isValidRole('Admin')).toBe(true);
    expect(isValidRole('Pharmacist')).toBe(true);
    expect(isValidRole('Cashier')).toBe(true);
    expect(isValidRole('Staff')).toBe(false);
    expect(isValidRole('Manager')).toBe(false);
  });

  it('defines permissions for Admin, Pharmacist, and Cashier', () => {
    expect(ROLE_PERMISSIONS.Admin).toContain('user:admin');
    expect(ROLE_PERMISSIONS.Admin).toContain('purchase-order:approve');

    expect(ROLE_PERMISSIONS.Pharmacist).toContain('inventory:write');
    expect(ROLE_PERMISSIONS.Pharmacist).not.toContain('user:admin');
    expect(ROLE_PERMISSIONS.Pharmacist).not.toContain('purchase-order:approve');

    expect(ROLE_PERMISSIONS.Cashier).toContain('sales:write');
    expect(ROLE_PERMISSIONS.Cashier).toContain('return:write');
    expect(ROLE_PERMISSIONS.Cashier).not.toContain('inventory:write');
    expect(ROLE_PERMISSIONS.Cashier).not.toContain('purchase-order:read');
  });

  it('evaluates role checks accurately', () => {
    const pharmacistRoles: Role[] = ['Pharmacist'];
    expect(hasRole(pharmacistRoles, 'Pharmacist')).toBe(true);
    expect(hasRole(pharmacistRoles, 'Admin')).toBe(false);
    expect(hasAnyRole(pharmacistRoles, ['Admin', 'Pharmacist'])).toBe(true);
    expect(hasAnyRole(pharmacistRoles, ['Admin', 'Cashier'])).toBe(false);
    expect(hasRole([], 'Pharmacist')).toBe(false);
  });

  it('evaluates permission checks accurately', () => {
    const scopes = ['inventory:read', 'inventory:write'] as const;
    expect(hasPermission(scopes, 'inventory:read')).toBe(true);
    expect(hasPermission(scopes, 'sales:write')).toBe(false);
    expect(
      hasAllPermissions(scopes, ['inventory:read', 'inventory:write'])
    ).toBe(true);
    expect(hasAllPermissions(scopes, ['inventory:read', 'sales:write'])).toBe(
      false
    );
  });
});
