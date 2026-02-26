import z from "zod";

export const RefreshTokenRequest = z.object({
  refreshToken: z.string().min(1, { message: "Refresh token is required" })
});

export type RefreshTokenRequest = z.infer<typeof RefreshTokenRequest>;
