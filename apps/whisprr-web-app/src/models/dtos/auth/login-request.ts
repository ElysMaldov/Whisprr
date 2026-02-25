import z from "zod";

export const LoginRequest = z.object({
  email: z.string().email({ message: "Please enter a valid email" }),
  password: z.string().min(1, { message: "This field is required" })
});

export type LoginRequest = z.infer<typeof LoginRequest>;
