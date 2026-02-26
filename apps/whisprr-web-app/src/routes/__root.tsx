import type { RouterContext } from "@/lib/router-context";
import { createRootRouteWithContext, Outlet } from "@tanstack/react-router";

const RootLayout = () => {
  return <Outlet />;
};

export const Route = createRootRouteWithContext<RouterContext>()({
  component: RootLayout
});
