import { QueryClientProvider } from "@tanstack/react-query";
import { RouterProvider } from "@tanstack/react-router";
import AuthRepository from "./data/repositories/auth";
import AuthService from "./data/services/auth";
import { appAxios } from "./lib/app-axios";
import { queryClient } from "./lib/query-client";
import { router } from "./router";

const InnerApp = () => {
  // Declare singletons
  const authService = new AuthService(appAxios);
  const authRepository = new AuthRepository(authService);

  return (
    <RouterProvider
      router={router}
      context={{ authRepository, queryClient }}
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
