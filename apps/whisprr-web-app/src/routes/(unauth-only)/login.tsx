import LoginScreen from "@/pages/LoginScreen";
import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/(unauth-only)/login")({
  component: RouteComponent,
  beforeLoad: ({ context, location }) => {
    if (context.auth.isAuthenticated) {
      throw redirect({
        to: "/dashboard",
        search: {
          redirect: location.href
        }
      });
    }
  }
});

function RouteComponent() {
  const { authRepository } = Route.useRouteContext();

  return <LoginScreen authRepository={authRepository} />;
}
