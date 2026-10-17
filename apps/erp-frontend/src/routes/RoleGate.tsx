import type { ReactNode } from 'react';
import { useAuthStore } from '../stores/authStore';
import { hasAnyRole } from '../lib/auth/permissions';
import type { Role } from '../types/api';

interface RoleGateProps {
  allowedRoles: readonly Role[];
  children: ReactNode;
  fallback?: ReactNode;
}

export function RoleGate({ allowedRoles, children, fallback }: RoleGateProps) {
  const currentUser = useAuthStore((state) => state.currentUser);
  const userRoles = currentUser?.roles;

  const isAllowed = hasAnyRole(userRoles, allowedRoles);

  if (!isAllowed) {
    if (fallback !== undefined) {
      return <>{fallback}</>;
    }

    return (
      <div
        data-testid="role-gate-forbidden"
        className="flex min-h-[50vh] flex-col items-center justify-center p-6 text-center"
      >
        <div className="max-w-md rounded-xl border border-red-800/40 bg-red-950/20 p-8 shadow-lg">
          <h2 className="text-xl font-bold text-red-400">
            403 Forbidden / Unauthorized
          </h2>
          <p className="mt-2 text-sm text-slate-300">
            You do not have the required permissions to view this section.
          </p>
          <div className="mt-4 text-xs text-slate-400">
            Allowed roles:{' '}
            <span className="font-semibold text-slate-200">
              {allowedRoles.join(', ')}
            </span>
          </div>
        </div>
      </div>
    );
  }

  return <>{children}</>;
}
