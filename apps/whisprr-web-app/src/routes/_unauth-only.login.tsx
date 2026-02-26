import LoginScreen from "@/pages/LoginScreen";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/_unauth-only/login")({
  component: RouteComponent
});

function RouteComponent() {
  return <LoginScreen />;
}
