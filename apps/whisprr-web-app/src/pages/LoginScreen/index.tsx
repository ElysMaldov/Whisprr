import { LoginForm } from "@/pages/LoginScreen/LoginForm";
import { Navbar } from "@/components/Navbar";
import type AuthRepository from "@/data/repositories/auth";

export interface LoginScreenProps {
  authRepository: AuthRepository;
}

const LoginScreen = ({ authRepository }: LoginScreenProps) => {
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
        <LoginForm authRepository={authRepository} />
      </main>
    </section>
  );
};

export default LoginScreen;
