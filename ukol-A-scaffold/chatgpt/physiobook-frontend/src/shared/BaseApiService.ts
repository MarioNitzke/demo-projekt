import type { ProblemDetailsResponse } from "./ApiResponse";

export class ApiError extends Error {
  public readonly status: number;
  public readonly problem?: ProblemDetailsResponse;

  constructor(message: string, status: number, problem?: ProblemDetailsResponse) {
    super(message);
    this.status = status;
    this.problem = problem;
  }
}

type HttpMethod = "GET" | "POST" | "PUT" | "DELETE";

export default class BaseApiService {
  private readonly baseUrl = import.meta.env.VITE_API_BASE_URL ?? "/api";

  private formatProblem(problem?: ProblemDetailsResponse): string {
    if (!problem) {
      return "Request failed.";
    }

    const validationMessages = problem.errors
      ? Object.entries(problem.errors)
          .flatMap(([field, messages]) => messages.map((message) => `${field}: ${message}`))
      : [];

    if (validationMessages.length > 0) {
      return validationMessages.join("\n");
    }

    return problem.detail ?? problem.title ?? "Request failed.";
  }

  protected async request<T>(path: string, method: HttpMethod, body?: unknown): Promise<T> {
    const token = localStorage.getItem("physiobook.accessToken");

    const response = await fetch(`${this.baseUrl}${path}`, {
      method,
      headers: {
        "Content-Type": "application/json",
        ...(token ? { Authorization: `Bearer ${token}` } : {})
      },
      body: body ? JSON.stringify(body) : undefined
    });

    if (!response.ok) {
      const text = await response.text();
      let problem: ProblemDetailsResponse | undefined;

      try {
        problem = text ? (JSON.parse(text) as ProblemDetailsResponse) : undefined;
      } catch {
        problem = undefined;
      }

      throw new ApiError(this.formatProblem(problem), response.status, problem);
    }

    if (response.status === 204) {
      return undefined as T;
    }

    return (await response.json()) as T;
  }

  protected get<T>(path: string): Promise<T> {
    return this.request<T>(path, "GET");
  }

  protected post<T>(path: string, body?: unknown): Promise<T> {
    return this.request<T>(path, "POST", body);
  }

  protected put<T>(path: string, body?: unknown): Promise<T> {
    return this.request<T>(path, "PUT", body);
  }

  protected delete<T>(path: string): Promise<T> {
    return this.request<T>(path, "DELETE");
  }
}
