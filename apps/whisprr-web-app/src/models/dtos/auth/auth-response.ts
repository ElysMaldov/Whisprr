import z from "zod";

export const AuthResponse = z.object({
  userId: z.string().uuid(),
  email: z.string().email(),
  displayName: z.string().optional(),
  token: z.string(),
  refreshToken: z.string()
});

export type AuthResponse = z.infer<typeof AuthResponse>;
