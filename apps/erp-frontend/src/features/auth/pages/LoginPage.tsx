import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../../../stores/authStore';
import { PATHS } from '../../../routes/paths';

export function LoginPage() {
  const login = useAuthStore((state) => state.login);
  const navigate = useNavigate();

  const handleQuickLogin = async () => {
    await login({
      organization_code: 'PHARMAPLUS',
      username: 'fa.pharmacist',
      password: 'password123',
    });
    navigate(PATHS.HOME);
  };

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-900 p-4 text-white">
      <div className="w-full max-w-sm rounded-xl border border-slate-800 bg-slate-950 p-6 text-center shadow-xl">
        <h1 className="text-2xl font-bold text-emerald-400">
          Login Screen Stub
        </h1>
        <p className="mt-2 text-sm text-slate-400">
          Enter credentials or click below for MSW fixture login
        </p>
        <button
          onClick={() => void handleQuickLogin()}
          className="mt-6 w-full cursor-pointer rounded-lg bg-emerald-600 px-4 py-2 font-medium text-white transition hover:bg-emerald-500"
        >
          Sign In (Demo / MSW)
        </button>
      </div>
    </div>
  );
}

export default LoginPage;
