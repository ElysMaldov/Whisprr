import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useAuthStore } from "@/store/auth-store";
import { authKeys } from "./useLogin";

/**
 * Hook for logout mutation using TanStack Query.
 * Clears the auth store and removes cached auth queries.
 */
export function useLogout() {
  const queryClient = useQueryClient();
  const { clearUser } = useAuthStore();

  return useMutation({
    mutationFn: async () => {
      clearUser();
      queryClient.removeQueries({ queryKey: authKeys.all });
    }
  });
}
