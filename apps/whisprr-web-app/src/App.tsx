import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "@tanstack/react-router";
import { queryClient } from "./lib/query-client";
import { router } from "./router";
import { useAuthStore } from "./store/auth-store";

const InnerApp = () => {
  const auth = useAuthStore();

  return (
    <RouterProvider
      router={router}
      context={{ auth }}
    />
  );
};

const App = () => {
  return (
    <QueryClientProvider client={queryClient}>
      <InnerApp />
    </QueryClientProvider>
  );
};

export default App;
