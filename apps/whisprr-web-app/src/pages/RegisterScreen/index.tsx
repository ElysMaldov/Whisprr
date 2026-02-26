import { Navbar } from "@/components/Navbar";
import { RegisterForm } from "@/pages/RegisterScreen/RegisterForm";

export interface RegisterScreenProps {}

const RegisterScreen = () => {
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
        <RegisterForm />
      </main>
    </section>
  );
};

export default RegisterScreen;
