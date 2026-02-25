import z from "zod";
import { PlatformTypeSchema } from "./platform-type";

export const SocialTopic = z.object({
  id: z.string().uuid(),
  description: z.string().optional(),
  keywords: z.array(z.string().min(1)),
  language: z.string(),
  platform: PlatformTypeSchema
});

export type SocialTopic = z.infer<typeof SocialTopic>;
