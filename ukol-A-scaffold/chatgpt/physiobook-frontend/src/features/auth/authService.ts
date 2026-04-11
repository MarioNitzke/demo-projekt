import BaseApiService from "../../shared/BaseApiService";
import type {
  AuthResponse,
  LoginRequest,
  RefreshTokenRequest,
  RegisterRequest
} from "./authTypes";

class AuthService extends BaseApiService {
  login(request: LoginRequest): Promise<AuthResponse> {
    return this.post<AuthResponse>("/auth/login", request);
  }

  register(request: RegisterRequest): Promise<AuthResponse> {
    return this.post<AuthResponse>("/auth/register", request);
  }

  refreshToken(request: RefreshTokenRequest): Promise<AuthResponse> {
    return this.post<AuthResponse>("/auth/refresh-token", request);
  }
}

const authService = new AuthService();
export default authService;
