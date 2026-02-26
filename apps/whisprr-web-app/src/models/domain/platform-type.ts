import z from "zod";

export enum PlatformType {
  Bluesky = "bluesky",
  Mastodon = "mastodon"
}

export const PlatformTypeSchema = z.nativeEnum(PlatformType);
