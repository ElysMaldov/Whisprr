import { AuthResponse } from "@/models/dtos/auth/auth-response";
import type { LoginRequest } from "@/models/dtos/auth/login-request";
import type { RefreshTokenRequest } from "@/models/dtos/auth/refresh-token-request";
import type { RegisterRequest } from "@/models/dtos/auth/register-request";
import type { AxiosInstance } from "axios";

export default class AuthService {
  private axios: AxiosInstance;

  constructor(axios: AxiosInstance) {
    this.axios = axios;
  }

  /**
   * Authenticates a user and returns tokens.
   * @param request - The login request containing email and password
   * @returns Promise with AuthResponse containing tokens and user info
   */
  public async login(request: LoginRequest): Promise<AuthResponse> {
    const response = await this.axios.post("/auth/login", request);
    return AuthResponse.parse(response.data);
  }

  /**
   * Registers a new user.
   * @param request - The registration request containing email, password, and optional display name
   * @returns Promise with AuthResponse containing tokens and user info
   */
  public async register(request: RegisterRequest): Promise<AuthResponse> {
    const response = await this.axios.post("/auth/register", request);
    return AuthResponse.parse(response.data);
  }

  /**
   * Refreshes an access token using a valid refresh token.
   * @param request - The refresh token request containing the refresh token
   * @returns Promise with AuthResponse containing new tokens and user info
   */
  public async refreshToken(
    request: RefreshTokenRequest
  ): Promise<AuthResponse> {
    const response = await this.axios.post("/auth/refresh", request);
    return AuthResponse.parse(response.data);
  }
}
