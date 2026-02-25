import { createRouter } from "@tanstack/react-router";
import { routeTree } from "./routeTree.gen";

export const router = createRouter({
  routeTree,
  // These will be populate in App.tsx
  context: {
    authRepository: undefined!,
    queryClient: undefined!,
    auth: undefined!
  }
});

// Register the router instance for type safety
declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}
