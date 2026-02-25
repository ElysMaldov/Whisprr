import RegisterScreen from "@/pages/RegisterScreen";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/register")({
  component: RouteComponent
});

function RouteComponent() {
  const { authRepository } = Route.useRouteContext();

  return <RegisterScreen authRepository={authRepository} />;
}
