import z from "zod";

export const RegisterRequest = z.object({
  email: z.string().email({ message: "Please enter a valid email" }),
  password: z.string().min(8, { message: "Password must be at least 8 characters" }),
  displayName: z.string().max(200).optional()
});

export type RegisterRequest = z.infer<typeof RegisterRequest>;
