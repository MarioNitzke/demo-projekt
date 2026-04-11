import { FormEvent, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { useToast } from '../contexts/ToastContext';

export function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();
  const { showToast } = useToast();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');

  const onSubmit = async (e: FormEvent) => {
    e.preventDefault();
    try {
      await login({ email, password });
      showToast('Logged in.');
      navigate('/articles');
    } catch (error) {
      showToast((error as Error).message);
    }
  };

  return (
    <form onSubmit={onSubmit} className="mx-auto max-w-md space-y-3 rounded border bg-white p-4">
      <h1 className="text-xl font-semibold">Login</h1>
      <input className="w-full rounded border p-2" placeholder="Email" value={email} onChange={(e) => setEmail(e.target.value)} />
      <input className="w-full rounded border p-2" placeholder="Password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
      <button className="rounded bg-slate-900 px-4 py-2 text-white" type="submit">
        Sign in
      </button>
    </form>
  );
}

