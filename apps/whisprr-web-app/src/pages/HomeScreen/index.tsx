import Hero from "@/components/Hero";
import { Navbar } from "@/components/Navbar";

export interface HomeScreenProps {}

const HomeScreen = ({}: HomeScreenProps) => {
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
      <Hero className="w-full flex items-center justify-center flex-1" />
    </section>
  );
};

export default HomeScreen;
