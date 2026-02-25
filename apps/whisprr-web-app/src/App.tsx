import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Outlet } from "@tanstack/react-router";

export interface AppProps {
  queryClient: QueryClient;
}

const App = ({ queryClient }: AppProps) => {
  return (
    <QueryClientProvider client={queryClient}>
      <Outlet />
      {/* <ReactQueryDevtools initialIsOpen={false} /> */}
    </QueryClientProvider>
  );
};

export default App;
