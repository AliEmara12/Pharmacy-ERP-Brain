import { useAuthStore } from '../../../stores/authStore';

export function DashboardPage() {
  const currentUser = useAuthStore((state) => state.currentUser);
  const logout = useAuthStore((state) => state.logout);

  return (
    <div className="flex min-h-screen flex-col items-center justify-center bg-slate-900 p-6 text-white">
      <div className="w-full max-w-lg rounded-xl border border-slate-800 bg-slate-950 p-8 shadow-2xl">
        <div className="flex items-center justify-between border-b border-slate-800 pb-4">
          <h1 className="text-xl font-bold text-emerald-400">
            Dashboard Stub (Protected)
          </h1>
          <button
            onClick={() => void logout()}
            className="cursor-pointer rounded-md bg-red-600/80 px-3 py-1.5 text-xs font-semibold text-white transition hover:bg-red-500"
          >
            Logout
          </button>
        </div>

        <div className="mt-6 space-y-3 text-sm text-slate-300">
          <div>
            <span className="text-slate-400">Username / Operator: </span>
            <span className="font-mono font-semibold text-emerald-300">
              {currentUser?.user.username || 'N/A'}
            </span>
          </div>
          <div>
            <span className="text-slate-400">Role: </span>
            <span className="rounded bg-slate-800 px-2 py-0.5 font-semibold text-emerald-400">
              {currentUser?.user.role || currentUser?.roles.join(', ') || 'N/A'}
            </span>
          </div>
          <div>
            <span className="text-slate-400">Organization: </span>
            <span className="text-slate-200">
              {currentUser?.organization_code || 'N/A'}
            </span>
          </div>
        </div>
      </div>
    </div>
  );
}

export default DashboardPage;
