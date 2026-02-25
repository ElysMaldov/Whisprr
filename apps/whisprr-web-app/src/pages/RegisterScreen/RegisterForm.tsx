"use client";
import { Password } from "@/components/password";
import { Button } from "@/components/ui/button";
import {
  Field,
  FieldContent,
  FieldError,
  FieldGroup,
  FieldLabel
} from "@/components/ui/field";
import { Input } from "@/components/ui/input";
import { zodResolver } from "@hookform/resolvers/zod";
import { Controller, useForm } from "react-hook-form";
import * as z from "zod";
import { RegisterSchema } from "./RegisterSchema";
import { useNavigate } from "@tanstack/react-router";
import { authRepository } from "@/data/repositories/auth";

type Schema = z.infer<typeof RegisterSchema>;

export interface RegisterFormProps {}

export function RegisterForm({}: RegisterFormProps) {
  const navigate = useNavigate();
  const form = useForm<Schema>({
    resolver: zodResolver(RegisterSchema),
    defaultValues: {
      email: "",
      password: "",
      confirmPassword: "",
      displayName: ""
    }
  });

  const {
    formState: { isSubmitting }
  } = form;

  const handleSubmit = form.handleSubmit(async (data: Schema) => {
    try {
      await authRepository.register({
        email: data.email,
        password: data.password,
        displayName: data.displayName
      });
      form.reset();
      navigate({ to: "/login" });
    } catch (error) {
      console.error(error);
    }
  });

  return (
    <form
      onSubmit={handleSubmit}
      className="p-2 sm:p-5 md:p-8 w-full rounded-md gap-2 border max-w-3xl mx-auto"
    >
      <FieldGroup className="grid md:grid-cols-6 gap-4 mb-6">
        <h1 className="mt-6 mb-1 font-extrabold text-3xl tracking-tight col-span-full">
          Register
        </h1>
        <p className="tracking-wide text-muted-foreground mb-5 text-wrap text-sm col-span-full">
          Create a new account to get started
        </p>

        <Controller
          name="displayName"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field
              data-invalid={fieldState.invalid}
              className="gap-1 col-span-full"
            >
              <FieldLabel htmlFor="displayName">Display Name </FieldLabel>
              <Input
                {...field}
                id="displayName"
                type="text"
                onChange={(e) => {
                  field.onChange(e.target.value);
                }}
                aria-invalid={fieldState.invalid}
                placeholder="Enter your display name (optional)"
              />

              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />

        <Controller
          name="email"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field
              data-invalid={fieldState.invalid}
              className="gap-1 col-span-full"
            >
              <FieldLabel htmlFor="email">Email </FieldLabel>
              <Input
                {...field}
                id="email"
                type="text"
                onChange={(e) => {
                  field.onChange(e.target.value);
                }}
                aria-invalid={fieldState.invalid}
                placeholder="Enter your Email"
              />

              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />

        <Controller
          name="password"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field
              data-invalid={fieldState.invalid}
              className="gap-1 col-span-full"
            >
              <FieldContent className="gap-0.5">
                <FieldLabel htmlFor="password">Password</FieldLabel>
              </FieldContent>
              <Password
                {...field}
                aria-invalid={fieldState.invalid}
                id="password"
                placeholder="Password"
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />

        <Controller
          name="confirmPassword"
          control={form.control}
          render={({ field, fieldState }) => (
            <Field
              data-invalid={fieldState.invalid}
              className="gap-1 col-span-full"
            >
              <FieldContent className="gap-0.5">
                <FieldLabel htmlFor="confirmPassword">
                  Confirm Password
                </FieldLabel>
              </FieldContent>
              <Password
                {...field}
                aria-invalid={fieldState.invalid}
                id="confirmPassword"
                placeholder="Confirm your password"
              />
              {fieldState.invalid && <FieldError errors={[fieldState.error]} />}
            </Field>
          )}
        />
      </FieldGroup>
      <div className="flex justify-end items-center w-full">
        <Button
          disabled={isSubmitting}
          type="submit"
        >
          {isSubmitting ? "Submitting..." : "Register"}
        </Button>
      </div>
    </form>
  );
}
