import { Navbar } from "@/components/Navbar";
import { LoginForm } from "@/pages/LoginScreen/LoginForm";

export interface LoginScreenProps {}

const LoginScreen = ({}: LoginScreenProps) => {
  return (
    <section className="flex flex-col w-full min-h-screen">
      <Navbar
        className="w-full flex justify-center"
        auth={{
          login: {
            title: "Login",
            url: "/login"
          },
          signup: {
            title: "Register",
            url: "/register"
          }
        }}
      />
      <main className="flex flex-1 items-center justify-center">
        <LoginForm />
      </main>
    </section>
  );
};

export default LoginScreen;
