import Hero from "@/components/Hero";
import { Navbar1 } from "@/components/Navbar";

export interface HomeScreenProps {}

const HomeScreen = ({}: HomeScreenProps) => {
  return (
    <section className="flex flex-col w-full min-h-screen">
      <Navbar1 className="w-full flex justify-center" />
      <Hero className="w-full flex items-center justify-center flex-1" />
    </section>
  );
};

export default HomeScreen;
