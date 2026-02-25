import App from "@/App";
import type { RouterContext } from "@/lib/router-context";
import { createRootRouteWithContext } from "@tanstack/react-router";

const RootLayout = () => {
  const { queryClient } = Route.useRouteContext();

  return <App queryClient={queryClient} />;
};

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootLayout
});
