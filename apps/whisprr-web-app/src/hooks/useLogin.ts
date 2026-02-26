import { useMutation, useQueryClient } from "@tanstack/react-query";
import { authRepository } from "@/data/repositories/auth";
import { useAuthStore } from "@/store/auth-store";
import type { LoginRequest } from "@/models/dtos/auth/login-request";

/**
 * Query keys for auth-related queries
 */
export const authKeys = {
  all: ["auth"] as const
};

/**
 * Hook for login mutation using TanStack Query.
 * Automatically updates the auth store on successful login.
 */
export function useLogin() {
  const queryClient = useQueryClient();
  const { setUser } = useAuthStore();

  return useMutation({
    mutationFn: (request: LoginRequest) => authRepository.login(request),
    onSuccess: (user) => {
      setUser(user);
      queryClient.invalidateQueries({ queryKey: authKeys.all });
    }
  });
}
