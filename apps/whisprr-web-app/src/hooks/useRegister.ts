import { useMutation, useQueryClient } from "@tanstack/react-query";
import { authRepository } from "@/data/repositories/auth";
import { useAuthStore } from "@/store/auth-store";
import type { RegisterRequest } from "@/models/dtos/auth/register-request";
import { authKeys } from "./useLogin";

/**
 * Hook for register mutation using TanStack Query.
 * Automatically updates the auth store on successful registration.
 */
export function useRegister() {
  const queryClient = useQueryClient();
  const { setUser } = useAuthStore();

  return useMutation({
    mutationFn: (request: RegisterRequest) => authRepository.register(request),
    onSuccess: (user) => {
      setUser(user);
      queryClient.invalidateQueries({ queryKey: authKeys.all });
    }
  });
}
