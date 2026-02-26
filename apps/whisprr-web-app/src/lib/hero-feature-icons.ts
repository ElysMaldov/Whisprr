import type { ElementType } from "react";

export interface Feature {
  title: string;
  description: string;
  icon: ElementType<{ className?: string }>;
  color?: string;
  href?: string;
}

export interface Image {
  src: string;
  alt: string;
}

export interface HeroFeatureIconsProps {
  badge?: string;
  heading: string;
  description?: string;
  buttonPrimary?: {
    text: string;
    href: string;
  };
  buttonSecondary?: {
    text: string;
    href: string;
  };
  features?: Feature[];
  /** Array of images (at least 1 required). Multiple images enable carousel behavior. */
  images: [Image, ...Image[]];
  className?: string;
}
