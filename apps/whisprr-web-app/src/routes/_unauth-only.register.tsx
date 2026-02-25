import RegisterScreen from "@/pages/RegisterScreen";
import { createFileRoute, redirect } from "@tanstack/react-router";

export const Route = createFileRoute("/_unauth-only/register")({
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

  return <RegisterScreen authRepository={authRepository} />;
}
