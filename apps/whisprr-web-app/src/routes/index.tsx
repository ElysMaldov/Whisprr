import { ComponentExample } from "@/components/component-example";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({
  component: Index
});

function Index() {
  return <ComponentExample />;
}
