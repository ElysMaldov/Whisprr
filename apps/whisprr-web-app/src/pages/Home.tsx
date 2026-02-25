import { Hero7 } from "@/components/Hero";
import { Navbar1 } from "@/components/Navbar";

export interface HomeProps {}

const Home = ({}: HomeProps) => {
  return (
    <section className="flex  flex-col min-h-screen">
      <Navbar1 className="grow-0" />
      <Hero7 className="flex flex-1 items-center" />
    </section>
  );
};

export default Home;
