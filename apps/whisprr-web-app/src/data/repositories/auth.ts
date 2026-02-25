import { User as UserSchema } from "@/models/domain/user";
import type { LoginRequest } from "@/models/dtos/auth/login-request";
import type { RefreshTokenRequest } from "@/models/dtos/auth/refresh-token-request";
import type { RegisterRequest } from "@/models/dtos/auth/register-request";
import type AuthService from "../services/auth";
import type { User } from "@/models/domain/user";

export default class AuthRepository {
  private authService: AuthService;

  constructor(authService: AuthService) {
    this.authService = authService;
  }

  public async login(request: LoginRequest): Promise<User> {
    const response = await this.authService.login(request);
    return UserSchema.parse(response);
  }

  public async register(request: RegisterRequest): Promise<User> {
    const response = await this.authService.register(request);
    return UserSchema.parse(response);
  }

  public async refreshToken(request: RefreshTokenRequest): Promise<User> {
    const response = await this.authService.refreshToken(request);
    return UserSchema.parse(response);
  }
}
