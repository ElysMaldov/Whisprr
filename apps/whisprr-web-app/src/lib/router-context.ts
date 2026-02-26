import type { AuthState } from "@/store/auth-store";
import type { QueryClient } from "@tanstack/react-query";
import type { AuthRepository } from "@/data/repositories/auth";

/**
 * Router Context Interface
 *
 * This interface defines the shape of the context passed through TanStack Router.
 * The QueryClient is provided via DI (Dependency Injection) from the root route
 * and is accessible throughout the entire route tree.
 *
 * @example
 * // Access queryClient in a component
 * const { queryClient } = useRouteContext({ from: "__root__" });
 *
 * @example
 * // Access queryClient in a route loader
 * export const Route = createFileRoute("/users")({
 *   loader: async ({ context }) => {
 *     const data = await context.queryClient.fetchQuery({
 *       queryKey: ["users"],
 *       queryFn: fetchUsers,
 *     });
 *     return { data };
 *   },
 *   component: UsersComponent,
 * });
 */
export interface RouterContext {
  auth: AuthState;
  queryClient: QueryClient;
  authRepository: AuthRepository;
}
