import { Separator } from "@/components/ui/separator";
import { cn } from "@/lib/utils";
import { HandHelping, Users } from "lucide-react";
import React from "react";
import { Button } from "./ui/button";

export interface HeroProps {
  className?: string;
}

const Hero = ({ className }: HeroProps) => {
  const features = [
    {
      icon: HandHelping,
      title: "100 posts collected",
      description:
        "Benefit from around-the-clock assistance to keep your business running smoothly."
    },
    {
      icon: Users,
      title: "100 topics to listen to",
      description:
        "Enhance teamwork with tools designed to simplify project management and communication."
    }
  ];

  return (
    <section className={cn("py-32", className)}>
      <div className="container overflow-hidden">
        <div className="mb-20 flex flex-col items-center gap-6 text-center">
          <h1 className="text-4xl font-semibold lg:text-5xl">
            Listen to distributed social media
          </h1>
          <p className="text-xl max-w-lg">
            Get the latest updates from Bluesky and Mastodon for topics that are
            important to you.
          </p>
          <div className="w-full flex justify-center">
            <Button
              size="xl"
              render={<a href={"/login"} />}
            >
              Listen now
            </Button>
          </div>
        </div>

        <div className="mx-auto mt-10 flex max-w-5xl flex-col md:flex-row">
          {features.map((feature, index) => (
            <React.Fragment key={feature.title}>
              {index > 0 && (
                <Separator
                  orientation="vertical"
                  className="mx-6 hidden h-auto w-0.5 bg-linear-to-b from-muted via-transparent to-muted md:block"
                />
              )}
              <div
                key={index}
                className="flex grow basis-0 flex-col rounded-md bg-background p-4"
              >
                <div className="mb-6 flex size-10 items-center justify-center rounded-full bg-background drop-shadow-lg">
                  <feature.icon className="h-auto w-5" />
                </div>
                <h3 className="mb-2 font-semibold">{feature.title}</h3>
                <p className="text-sm text-muted-foreground">
                  {feature.description}
                </p>
              </div>
            </React.Fragment>
          ))}
        </div>
      </div>
    </section>
  );
};

export default Hero;
