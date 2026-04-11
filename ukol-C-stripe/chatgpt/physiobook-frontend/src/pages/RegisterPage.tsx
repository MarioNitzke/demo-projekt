import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { useToast } from '@/contexts/ToastContext';
import Layout from '@/components/Layout';
import type { ProblemDetails } from '@/shared/ApiResponse';
import axios from 'axios';

export default function RegisterPage() {
  const [form, setForm] = useState({
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
  });
  const [loading, setLoading] = useState(false);
  const [errors, setErrors] = useState<Record<string, string[]>>({});
  const [generalError, setGeneralError] = useState('');

  const { register } = useAuth();
  const { showToast } = useToast();
  const navigate = useNavigate();

  const updateField = (field: string) => (e: React.ChangeEvent<HTMLInputElement>) => {
    setForm((prev) => ({ ...prev, [field]: e.target.value }));
  };

  const validate = (): boolean => {
    const newErrors: Record<string, string[]> = {};

    if (!form.firstName.trim()) newErrors.firstName = ['First name is required'];
    if (!form.lastName.trim()) newErrors.lastName = ['Last name is required'];
    if (!form.email.trim()) newErrors.email = ['Email is required'];
    if (!form.password.trim()) {
      newErrors.password = ['Password is required'];
    } else if (form.password.length < 6) {
      newErrors.password = ['Password must be at least 6 characters'];
    }
    if (form.password !== form.confirmPassword) {
      newErrors.confirmPassword = ['Passwords do not match'];
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setGeneralError('');

    if (!validate()) return;

    setLoading(true);
    try {
      await register(form.email, form.password, form.firstName, form.lastName);
      showToast('Registration successful! Please sign in.', 'success');
      navigate('/login');
    } catch (err: unknown) {
      if (axios.isAxiosError(err) && err.response?.data) {
        const problem = err.response.data as ProblemDetails;
        if (problem.errors) {
          setErrors(problem.errors);
        } else if (problem.detail) {
          setGeneralError(problem.detail);
        } else {
          setGeneralError('Registration failed. Please try again.');
        }
      } else {
        setGeneralError('An unexpected error occurred. Please try again.');
      }
    } finally {
      setLoading(false);
    }
  };

  const inputClass = (field: string) =>
    `w-full px-4 py-2.5 border rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-500 focus:border-transparent transition-shadow ${
      errors[field] ? 'border-red-300 bg-red-50' : 'border-gray-300'
    }`;

  return (
    <Layout>
      <div className="max-w-md mx-auto mt-8">
        <div className="bg-white rounded-xl shadow-sm border border-gray-200 p-8">
          <div className="text-center mb-8">
            <h1 className="text-2xl font-bold text-gray-900">Create Account</h1>
            <p className="text-gray-500 mt-2">Join PhysioBook today</p>
          </div>

          {generalError && (
            <div className="mb-6 p-4 bg-red-50 border border-red-200 rounded-lg text-red-700 text-sm">
              {generalError}
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label htmlFor="firstName" className="block text-sm font-medium text-gray-700 mb-1">
                  First Name
                </label>
                <input
                  id="firstName"
                  type="text"
                  value={form.firstName}
                  onChange={updateField('firstName')}
                  className={inputClass('firstName')}
                  placeholder="John"
                />
                {errors.firstName?.map((msg, i) => (
                  <p key={i} className="mt-1 text-sm text-red-600">{msg}</p>
                ))}
              </div>
              <div>
                <label htmlFor="lastName" className="block text-sm font-medium text-gray-700 mb-1">
                  Last Name
                </label>
                <input
                  id="lastName"
                  type="text"
                  value={form.lastName}
                  onChange={updateField('lastName')}
                  className={inputClass('lastName')}
                  placeholder="Doe"
                />
                {errors.lastName?.map((msg, i) => (
                  <p key={i} className="mt-1 text-sm text-red-600">{msg}</p>
                ))}
              </div>
            </div>

            <div>
              <label htmlFor="email" className="block text-sm font-medium text-gray-700 mb-1">
                Email Address
              </label>
              <input
                id="email"
                type="email"
                value={form.email}
                onChange={updateField('email')}
                className={inputClass('email')}
                placeholder="you@example.com"
              />
              {errors.email?.map((msg, i) => (
                <p key={i} className="mt-1 text-sm text-red-600">{msg}</p>
              ))}
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium text-gray-700 mb-1">
                Password
              </label>
              <input
                id="password"
                type="password"
                value={form.password}
                onChange={updateField('password')}
                className={inputClass('password')}
                placeholder="At least 6 characters"
              />
              {errors.password?.map((msg, i) => (
                <p key={i} className="mt-1 text-sm text-red-600">{msg}</p>
              ))}
            </div>

            <div>
              <label
                htmlFor="confirmPassword"
                className="block text-sm font-medium text-gray-700 mb-1"
              >
                Confirm Password
              </label>
              <input
                id="confirmPassword"
                type="password"
                value={form.confirmPassword}
                onChange={updateField('confirmPassword')}
                className={inputClass('confirmPassword')}
                placeholder="Repeat your password"
              />
              {errors.confirmPassword?.map((msg, i) => (
                <p key={i} className="mt-1 text-sm text-red-600">{msg}</p>
              ))}
            </div>

            <button
              type="submit"
              disabled={loading}
              className="w-full py-2.5 px-4 bg-teal-600 text-white font-medium rounded-lg hover:bg-teal-700 focus:outline-none focus:ring-2 focus:ring-teal-500 focus:ring-offset-2 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
              {loading ? (
                <span className="inline-flex items-center gap-2">
                  <svg
                    className="animate-spin h-4 w-4"
                    xmlns="http://www.w3.org/2000/svg"
                    fill="none"
                    viewBox="0 0 24 24"
                  >
                    <circle
                      className="opacity-25"
                      cx="12"
                      cy="12"
                      r="10"
                      stroke="currentColor"
                      strokeWidth="4"
                    />
                    <path
                      className="opacity-75"
                      fill="currentColor"
                      d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
                    />
                  </svg>
                  Creating account...
                </span>
              ) : (
                'Create Account'
              )}
            </button>
          </form>

          <p className="mt-6 text-center text-sm text-gray-500">
            Already have an account?{' '}
            <Link to="/login" className="text-teal-600 hover:text-teal-700 font-medium">
              Sign in here
            </Link>
          </p>
        </div>
      </div>
    </Layout>
  );
}
