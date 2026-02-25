import Hero from "@/components/Hero";

export interface HomeScreenProps {}

const HomeScreen = ({}: HomeScreenProps) => {
  return (
    <section className="flex flex-col w-full min-h-screen">
      <nav></nav>
      <Hero className="w-full flex items-center justify-center flex-1" />
    </section>
  );
};

export default HomeScreen;
