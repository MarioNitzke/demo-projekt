const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5050/api';

function authHeader(): Record<string, string> {
  const raw = localStorage.getItem('physiobook_auth');
  if (!raw) {
    return {};
  }

  try {
    const data = JSON.parse(raw) as { accessToken?: string };
    if (!data.accessToken) {
      return {};
    }

    return { Authorization: `Bearer ${data.accessToken}` };
  } catch {
    return {};
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const body = (await response.json().catch(() => ({}))) as { title?: string; detail?: string };
    throw new Error(body.detail ?? body.title ?? 'Request failed.');
  }

  if (response.status === 204) {
    return undefined as T;
  }

  return (await response.json()) as T;
}

export class BaseApiService {
  static async get<T>(path: string): Promise<T> {
    const response = await fetch(`${API_URL}${path}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        ...authHeader()
      }
    });

    return handleResponse<T>(response);
  }

  static async post<T>(path: string, body?: unknown): Promise<T> {
    const response = await fetch(`${API_URL}${path}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        ...authHeader()
      },
      body: body ? JSON.stringify(body) : undefined
    });

    return handleResponse<T>(response);
  }

  static async put<T>(path: string, body: unknown): Promise<T> {
    const response = await fetch(`${API_URL}${path}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        ...authHeader()
      },
      body: JSON.stringify(body)
    });

    return handleResponse<T>(response);
  }

  static async delete<T>(path: string): Promise<T> {
    const response = await fetch(`${API_URL}${path}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        ...authHeader()
      }
    });

    return handleResponse<T>(response);
  }
}

