import z from "zod";

export const User = z.object({
  userId: z.string().uuid(),
  email: z.string().email(),
  displayName: z.string().optional(),
  token: z.string(),
  refreshToken: z.string()
});

export type User = z.infer<typeof User>;
